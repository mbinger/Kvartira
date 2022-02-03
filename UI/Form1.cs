using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Providers;
using Providers.Degewo;
using Providers.Gewobag;
using Providers.ImmobilienScout24;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        private AppConfig appConfig;
        private Log log;
        private IProvider[] providers;
        public Director director;
        private Random rnd = new Random();
        private int queriesCount = 0;
        private DateTime NextQuery;
        private DateTime StartLoading;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                const string configFileName = "Config.json";
                string configContent;
                var path1 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Config", configFileName);
                var path2 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), configFileName);
                if (File.Exists(path1))
                {
                    configContent = File.ReadAllText(path1);
                }
                else if (File.Exists(path2))
                {
                    configContent = File.ReadAllText(path2);
                }
                else throw new Exception($"Configuration file not found:\n{path1}\n{path2}");

                appConfig = JsonConvert.DeserializeObject<AppConfig>(configContent);
                WohnungDb.AppConfig = appConfig;

                using (var context = new WohnungDb())
                {
                    context.Database.Migrate();
                }

                StatusToolStripComboBox.SelectedIndex = appConfig.FilterVisitedInitialValue ?? 0;
                wbsComboBox.SelectedIndex = appConfig.FilterWbsInitialValue ?? 0;
                toolStripTextBoxRating.Text = (appConfig.FilterImportanceInitialValue ?? 1).ToString();

                log = new Log(appConfig);

                var downloader = new HttpDownloader(log, appConfig);
                var browserDownloader = new BrowserDownloader(log, appConfig);

                providers = new IProvider[]
                {
                new GewobagProvider(downloader, log),
                new DegewoProvider(downloader, log),
                new ImmobilienScout24Provider(browserDownloader, log)
                };

                director = new Director(appConfig, providers, log);

                //refresh
                RefreshGrid();

                EnableTimer();

                ShowProvidersRating();

                if (appConfig.RunMinimized)
                {
                    WindowState = FormWindowState.Minimized;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid(null);
        }

        public void RefreshGrid(List<Guid> requiredIds = null)
        {
            var selectedIds = GetSelectedIds();

            dataGridView1.Rows.Clear();

            List<WohnungEntity> headers;

            using (var db = new WohnungDb())
            {
                var query = db.Wohnungen.AsQueryable();
                
                if (requiredIds?.Any() == true)
                {
                    query = query.Where(p => requiredIds.Contains(p.Id));
                }else
                {
                    var importance = Scanner.ParseInt(toolStripTextBoxRating.Text) ?? 0;
                    toolStripTextBoxRating.Text = importance.ToString();

                    int? rooms = null;
                    if (!string.IsNullOrEmpty(roomsTextBox.Text))
                    {
                        rooms = Scanner.ParseInt(roomsTextBox.Text);
                        if (rooms == null)
                        {
                            roomsTextBox.Text = "";
                        }
                    }

                    query = query.Where(p => p.Wichtigkeit >= importance);
                    if (StatusToolStripComboBox.SelectedIndex == 0)
                    {
                        //только новые
                        query = query.Where(p => p.Gesehen == null);
                    }
                    if (wbsComboBox.SelectedIndex == 0)
                    {
                        //без WBS
                        query = query.Where(p => p.Wbs == null || p.Wbs == false);
                    }
                    if (rooms != null)
                    {
                        //кол-во комнат
                        query = query.Where(p => p.Zimmer == null || p.Zimmer >= rooms.Value);
                    }
                }

                headers = query.ToList();
                var headerIds = headers.Select(p => p.Id).ToList();
            }

            var rows = new List<DataGridViewRow>();

            string formatBool(bool? val)
            {
                if (val == null)
                {
                    return "?";
                };
                return val.Value ? "Да" : "";
            }

            foreach (var header in headers)
            {
                dataGridView1.Rows.Add(header.Id,
                    header.WohnungId,
                    header.Provider,
                    header.Zimmer?.ToString() ?? "",
                    header.Flaeche?.ToString() ?? "",
                    header.MieteWarm?.ToString() ?? "",
                    formatBool(header.Wbs),
                    header.Bezirk,
                    header.Anschrift,
                    header.Ueberschrift,
                    header.Wichtigkeit,
                    formatBool(header.Balkon),
                    header.Gesehen?.ToString("dd.MM.yyyy"),
                    "открыть");
            }

            if (selectedIds.Any())
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Selected = false;
                    var id = GetId(row);
                    if (selectedIds.Contains(id))
                    {
                        row.Selected = true;
                    }
                }
            }

            dataGridView1.Refresh();
        }

        private void EnableTimer()
        {
            var interval = rnd.Next(appConfig.PoolingIntervalMin, appConfig.PoolingIntervalMax);
            NextQuery = DateTime.Now.AddMilliseconds(interval);
            nextQueryTimer.Enabled = true;
            poolingfTimer.Interval = interval;
            poolingfTimer.Enabled = true;
            startDownloadDataToolStripMenuItem.Enabled = true;
            loadingTimer.Enabled = false;
            loadingTimerValue.Visible = false;
            loadingTimerText.Visible = false;
            nextQueryText.Visible = true;
            nextQueryValue.Visible = true;
        }

        private int poolintTimerCommand = 0;
        private void poolingfTimer_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }

            poolingfTimer.Enabled = false;
            nextQueryTimer.Enabled = false;
            nextQueryValue.Text = "сейчас";
            startDownloadDataToolStripMenuItem.Enabled = false;
            StartLoading = DateTime.Now;
            loadingTimer.Enabled = true;
            nextQueryText.Visible = nextQueryValue.Visible = false;
            loadingTimerText.Visible = loadingTimerValue.Visible = true;

            backgroundWorker1.RunWorkerAsync();
        }

        private string FormatTimeSpan(TimeSpan diff)
        {
            if (diff.TotalMilliseconds < 0)
            {
                nextQueryValue.Text = "00:00";
                nextQueryTimer.Enabled = false;
                return null;
            }

            var seconds = diff.Seconds > 10
                ? diff.Seconds.ToString()
                : "0" + diff.Seconds.ToString();

            return $"{diff.Minutes}:{seconds}";
        }

        private void nextQueryTimer_Tick(object sender, EventArgs e)
        {
            var text = FormatTimeSpan(NextQuery - DateTime.Now);
            if (text == null)
            {
                nextQueryValue.Text = "00:00";
                nextQueryTimer.Enabled = false;
                return;
            }

            nextQueryValue.Text = text;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //start download
            try
            {
                switch (poolintTimerCommand)
                {
                    case 0:
                        director.LoadAsync().GetAwaiter().GetResult();
                        break;

                    case 1:
                        director.LoadAllDetailsAllAsync().GetAwaiter().GetResult();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                log.Write("ERROR pooling:\n" + ex.ToString());
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //download ends
            poolintTimerCommand = 0;
            queriesCount++;
            queriesCountLabel.Text = queriesCount.ToString();

            var oldIds = GetIdsFromCurrentView();

            //refresh view
            RefreshGrid();

            var newIds = GetIdsFromCurrentView();

            //loaded something new
            if (newIds.Any(p => !oldIds.Contains(p)))
            {
                WinApi.Flash(this);            
            }

            EnableTimer();

            ShowProvidersRating();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            var diff = FormatTimeSpan(DateTime.Now - StartLoading);
            if (diff == null)
            {
                loadingTimerValue.Text = "?";
                return;
            }
            loadingTimerValue.Text = diff;
        }

        private List<Guid> GetIdsFromCurrentView()
        {
            var result = new List<Guid>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var id = GetId(row);
                result.Add(id);
            }
            return result;
        }

        private Guid GetId(DataGridViewRow row)
        {
            var cellValue = row.Cells[IdColumn.Index].Value.ToString();
            var id = Guid.Parse(cellValue);
            return id;
        }

        private List<Guid> GetSelectedIds()
        {
            var result = new List<Guid>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Selected)
                {
                    var id = GetId(row);
                    result.Add(id);
                }
            }
            return result;
        }

        private void buttonMarkAllAsRead_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Пометить все как просмотренные?", "Вопрос", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            var ids = GetIdsFromCurrentView();

            using (var db = new WohnungDb())
            {
                var items = db.Wohnungen.Where(p => ids.Contains(p.Id) && p.Gesehen == null).ToList();
                foreach (var item in items)
                {
                    item.Gesehen = DateTime.Today;
                }
                db.SaveChanges();
            }

            RefreshGrid();
        }

        private void ShowProvidersRating()
        {
            var rating = director.GetRatingAll();
            switch (rating.Color)
            {
                case ProviderHealthColor.Red:
                    if (providerHealthButton.BackColor != Color.Red) providerHealthButton.BackColor = Color.Red;
                    break;

                case ProviderHealthColor.Yellow:
                    if (providerHealthButton.BackColor != Color.Yellow) providerHealthButton.BackColor = Color.Yellow;
                    break;

                case ProviderHealthColor.Gray:
                    if (providerHealthButton.BackColor != SystemColors.Control) providerHealthButton.BackColor = SystemColors.Control;
                    break;

                case ProviderHealthColor.Green:
                    if (providerHealthButton.BackColor != Color.DarkGreen) providerHealthButton.BackColor = Color.DarkGreen;
                    break;
            }
        }

        private void providerHealthButton_Click(object sender, EventArgs e)
        {
            var form = new ProvidersHealthForm(director);
            form.ShowDialog(this);
        }

        private void startDownloadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            poolintTimerCommand = 0; //загрузить данные
            poolingfTimer_Tick(sender, e);
        }

        private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                List<WohnungEntity> headers;

                using (var db = new WohnungDb())
                {
                    headers = db.Wohnungen.ToList();
                }

                var export = new List<WohnungExportItem>();
                foreach (var header in headers)
                {
                    var item = new WohnungExportItem
                    {
                        Provider = header.Provider,
                        WohnungId = header.WohnungId,
                        Wichtigkeit = header.Wichtigkeit,
                        SucheShort = header.SucheShort,
                        SucheDetails = header.SucheDetails,
                        Geladen = header.Geladen,
                        Gesehen = header.Gesehen,
                        Gemeldet = header.Gemeldet,
                        LoadDetailsTries = header.LoadDetailsTries,
                        Details = true
                    };

                    item.Ueberschrift = header.Ueberschrift;
                    item.Bezirk = header.Bezirk;
                    item.Anschrift = header.Anschrift;
                    item.MieteKalt = header.MieteKalt;
                    item.MieteWarm = header.MieteWarm;
                    item.Etage = header.Etage;
                    item.Etagen = header.Etagen;
                    item.Zimmer = header.Zimmer;
                    item.Flaeche = header.Flaeche;
                    item.FreiAb = header.FreiAb;
                    item.Beschreibung = header.Beschreibung;
                    item.Wbs = header.Wbs;
                    item.Balkon = header.Balkon;
                    item.Keller = header.Keller;

                    export.Add(item);
                }

                var fileName = saveFileDialog1.FileName;

                var content = JsonConvert.SerializeObject(export, Formatting.Indented);

                File.WriteAllText(fileName, content);

                Process.Start("notepad.exe", fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void importDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var content = File.ReadAllText(openFileDialog1.FileName);
                var import = JsonConvert.DeserializeObject<List<WohnungExportItem>>(content);
                var imported = 0;
                using (var db = new WohnungDb())
                {
                    foreach (var item in import)
                    {
                        var dbHeaders = db.Wohnungen.ToList();
                        var dbItem = dbHeaders.FirstOrDefault(p => p.Provider == item.Provider && p.WohnungId == item.WohnungId);
                        if (dbItem != null)
                        {
                            return;
                        }

                        imported++;

                        var header = new WohnungEntity
                        {
                            Provider = item.Provider,
                            WohnungId = item.WohnungId,
                            Wichtigkeit = item.Wichtigkeit,
                            SucheShort = item.SucheShort,
                            SucheDetails = item.SucheDetails,
                            Geladen = item.Geladen,
                            Gesehen = item.Gesehen,
                            Gemeldet = item.Gemeldet,
                            LoadDetailsTries = item.LoadDetailsTries,

                            Ueberschrift = item.Ueberschrift,
                            Bezirk = item.Bezirk,
                            Anschrift = item.Anschrift,
                            MieteKalt = item.MieteKalt,
                            MieteWarm = item.MieteWarm,
                            Etage = item.Etage,
                            Etagen = item.Etagen,
                            Zimmer = item.Zimmer,
                            Flaeche = item.Flaeche,
                            FreiAb = item.FreiAb,
                            Beschreibung = item.Beschreibung,
                            Wbs = item.Wbs,
                            Balkon = item.Balkon,
                            Keller = item.Keller
                        };

                        db.Wohnungen.Add(header);
                    }

                    db.SaveChanges();

                    MessageBox.Show($"{imported} записей импортировано");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                //open details

                //select current row if nothing selected
                foreach (DataGridViewRow row2 in dataGridView1.Rows)
                {
                    row2.Selected = false;
                }

                var row = dataGridView1.Rows[e.RowIndex];
                row.Selected = true;
                var id = GetId(row);
                var dlg = new Details(id, this);
                dlg.ShowDialog(this);
            }
            if (e.Button == MouseButtons.Right)
            {
                //context menu
                if (GetSelectedIds().Count() <= 1)
                {
                    //select current row if nothing selected
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.Selected = false;
                    }
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                }
                contextMenuStrip1.Show(this, Cursor.Position.X - this.Left, Cursor.Position.Y - this.Top);
            }
            if (e.Button == MouseButtons.Left && e.ColumnIndex == LinkColumn.Index)
            {
                //open link
                try
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    var id = GetId(row);
                    var provider = row.Cells[VermieterColumn.Index].Value.ToString();
                    var wohnungId = row.Cells[WohnungColumn.Index].Value.ToString();

                    OpenWohungInBrowser(provider, wohnungId);

                    using (var db = new WohnungDb())
                    {
                        var entry = db.Wohnungen.FirstOrDefault(p => p.Id == id);
                        if (entry != null)
                        {
                            entry.Gesehen = DateTime.Now;
                            db.SaveChanges();

                            //refresh
                            dataGridView1.Rows[e.RowIndex].Cells[GesehenColumn.Index].Value = entry.Gesehen?.ToString("dd.MM.yyyy");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void OpenWohungInBrowser(string provider, string wohnungId)
        {
            var url = director.GetOpenDetailsUrl(provider, wohnungId)?.TrimEnd();

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Ошибка получения URL");
            }

            OpenInBrowser(url);
        }

        public static void OpenInBrowser(string url)
        {
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = url;
            Process.Start(psi);
        }

        private void copyIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedIds();
            List<WohnungEntity> items;
            using (var db = new WohnungDb())
            {
                items = db.Wohnungen.Where(p => ids.Contains(p.Id)).ToList();
            }
            var msg = string.Join("; ", items.Select(p => $"{p.Provider} {p.WohnungId}"));
            Clipboard.SetText(msg);
        }

        private void загрузитьЗависшиеДеталиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            poolintTimerCommand = 1; //загрузить зависшие детали
            poolingfTimer_Tick(sender, e);
        }
    }
}
