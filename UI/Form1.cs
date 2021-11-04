using Common;
using Data;
using Newtonsoft.Json;
using Providers;
using Providers.Gewobag;
using System;
using System.Collections.Generic;
using System.Data;
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
        private Director director;
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
            StatusToolStripComboBox.SelectedIndex = 0;
            wbsComboBox.SelectedIndex = 0;

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
                context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
            }

            log = new Log(appConfig);

            var downloader = new HttpDownloader(log, appConfig);
            /*
            downloader = new DumpDownloader(appConfig, new[]
            {
                "2021_11_03_12_38_58_GEWOBAG_Test Gewobag load all page 1.htm",
                "2021_11_03_12_39_04_GEWOBAG_Test Gewobag load all page 2.htm"
            });
            */
            providers = new IProvider[]
            {
                new GewobagProvider(downloader, log)
            };

            director = new Director(appConfig, providers, log);

            //refresh
            toolStripButtonRefresh_Click(sender, e);

            EnableTimer();

            if (appConfig.RunMinimized)
            {
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            var importance = int.Parse(toolStripTextBoxRating.Text);
            dataGridView1.Rows.Clear();

            List<WohnungHeaderEntity> headers;
            List<WohnungDetailsEntity> details;

            using (var db = new WohnungDb())
            {
                var query = db.WohnungHeaders.Where(p => p.Wichtigkeit >= importance);
                if (StatusToolStripComboBox.SelectedIndex == 0)
                {
                    //только новые
                    query = query.Where(p => p.Gesehen == null);
                }
                if (wbsComboBox.SelectedIndex == 0)
                {
                    //без WBS
                    query = query.Where(p => !p.Details.Any(d => d.Wbs == true));
                }
                
                headers = query.ToList();
                var headerIds = headers.Select(p => p.Id).ToList();
                details = db.WohnungDetails.Where(p => headerIds.Contains(p.WohnungHeaderId)).ToList();
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
                var detail = details.FirstOrDefault(p => p.WohnungHeaderId == header.Id);
                dataGridView1.Rows.Add(header.Id,
                    header.WohnungId,
                    header.Provider,
                    detail?.Zimmer?.ToString() ?? "",
                    detail?.Flaeche?.ToString() ?? "",
                    detail?.MieteWarm?.ToString() ?? "",
                    formatBool(detail?.Wbs),
                    detail?.Bezirk,
                    detail?.Anschrift,
                    detail?.Ueberschrift,
                    header.Wichtigkeit, 
                    header.Gesehen?.ToString("dd.MM.yyyy"), 
                    "открыть");
            }

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == LinkColumn.Index)
            {
                try
                {
                    //open link
                    var id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[IdColumn.Index].Value.ToString());
                    var provider = dataGridView1.Rows[e.RowIndex].Cells[VermieterColumn.Index].Value.ToString();
                    var wohnungId = dataGridView1.Rows[e.RowIndex].Cells[WohnungColumn.Index].Value.ToString();

                    var url = director.GetOpenDetailsUrl(provider, wohnungId)?.TrimEnd();

                    if (string.IsNullOrEmpty(url))
                    {
                        MessageBox.Show("Невозможно открыть");
                    }

                    var psi = new System.Diagnostics.ProcessStartInfo();
                    psi.UseShellExecute = true;
                    psi.FileName = url;
                    System.Diagnostics.Process.Start(psi);

                    using (var db = new WohnungDb())
                    {
                        var entry = db.WohnungHeaders.FirstOrDefault(p => p.Id == id);
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

        private void EnableTimer()
        {
            var interval = rnd.Next(appConfig.PoolingIntervalMin, appConfig.PoolingIntervalMax);
            NextQuery = DateTime.Now.AddMilliseconds(interval);
            nextQueryTimer.Enabled = true;
            poolingfTimer.Interval = interval;
            poolingfTimer.Enabled = true;
            downloadNowButton.Enabled = true;
            loadingTimer.Enabled = false;
            loadingTimerValue.Visible = false;
            loadingTimerText.Visible = false;
            nextQueryText.Visible = true;
            nextQueryValue.Visible = true;
        }

        private void poolingfTimer_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }

            poolingfTimer.Enabled = false;
            nextQueryTimer.Enabled = false;
            nextQueryValue.Text = "сейчас";
            downloadNowButton.Enabled = false;
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

        private void downloadNowButton_Click(object sender, EventArgs e)
        {
            poolingfTimer_Tick(sender, e);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //start download
            try
            {
                var count = director.LoadAsync().GetAwaiter().GetResult();
                if (count > 0)
                {
                    WinApi.Flash(this);
                }
            }
            catch (Exception ex)
            {
                log.LogAsync("ERROR pooling:\n" + ex.ToString()).GetAwaiter().GetResult();
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //download ends
            queriesCount++;
            toolStripButtonRefresh_Click(sender, e);
            queriesCountLabel.Text = queriesCount.ToString();

            EnableTimer();
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

        private void buttonMarkAllAsRead_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Пометить все как просмотренные?", "Вопрос", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            var ids = dataGridView1.Rows.Cast<DataGridViewRow>().Select(p => int.Parse((string)p.Cells[IdColumn.Index].Value)).ToList();

            using (var db = new WohnungDb())
            {
                var items = db.WohnungHeaders.Where(p => ids.Contains(p.Id) && p.Gesehen == null).ToList();
                foreach (var item in items)
                {
                    item.Gesehen = DateTime.Today;
                }
                db.SaveChanges();
            }
        }
    }
}
