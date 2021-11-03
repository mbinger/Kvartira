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
using System.Threading;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        private AppConfig appConfig;
        private Log log;
        private IDownloader downloader;
        private IProvider[] providers;
        private Director director;
        private Random rnd = new Random();
        private int queriesCount = 0;
        private DateTime NextQuery;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StatusToolStripComboBox.SelectedIndex = 0;

            appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("Config.json"));
            WohnungDb.AppConfig = appConfig;

            using (var context = new WohnungDb())
            {
                context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
            }

            log = new Log(appConfig);

            //var downloader = new HttpDownloader(log, appConfig);

            downloader = new DumpDownloader(appConfig, new[]
            {
                "2021_11_03_12_38_58_GEWOBAG_Test Gewobag load all page 1.htm",
                "2021_11_03_12_39_04_GEWOBAG_Test Gewobag load all page 2.htm"
            });

            providers = new IProvider[]
            {
                new GewobagProvider(downloader, log)
            };

            director = new Director(appConfig, providers, log);

            //refresh
            toolStripButtonRefresh_Click(sender, e);

            EnableTimer();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            var importance = int.Parse(toolStripTextBoxRating.Text);
            dataGridView1.Rows.Clear();
            List<WohnungHeaderEntity> headers;
            using (var db = new WohnungDb())
            {
                var query = db.WohnungHeaders.Where(p => p.Wichtigkeit >= importance);
                if (StatusToolStripComboBox.SelectedIndex == 0)
                {
                    //только новые
                    query = query.Where(p => p.Gesehen == null);
                }
                headers = query.ToList();
            }

            var rows = new List<DataGridViewRow>();
            foreach (var header in headers)
            {
                dataGridView1.Rows.Add(header.Id, header.Provider, header.WohnungId, header.Wichtigkeit, header.Gesehen?.ToString("dd.MM.yyyy"), "открыть");
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
        }

        private void poolingfTimer_Tick(object sender, EventArgs e)
        {
            poolingfTimer.Enabled = false;
            nextQueryTimer.Enabled = false;
            nextQueryLabel.Text = "сейчас";

            try
            {
                var count = director.LoadAsync().GetAwaiter().GetResult();
                queriesCount++;
                if (count > 0)
                {
                    WinApi.Flash(this);

                    //refresh
                    toolStripButtonRefresh_Click(sender, e);
                }
                queriesCountLabel.Text = queriesCount.ToString();
            }
            catch (Exception ex)
            {
                log.LogAsync("ERROR pooling:\n" + ex.ToString()).GetAwaiter().GetResult();
            }

            EnableTimer();
        }

        private void nextQueryTimer_Tick(object sender, EventArgs e)
        {
            var diff = NextQuery - DateTime.Now;
            if (diff.TotalMilliseconds < 0)
            {
                nextQueryLabel.Text = "00:00";
                nextQueryTimer.Enabled = false;
                return;
            }

            var seconds = diff.Seconds > 10
                ? diff.Seconds.ToString()
                : "0" + diff.Seconds.ToString();

            nextQueryLabel.Text = $"{diff.Minutes}:{seconds}";
        }
    }
}
