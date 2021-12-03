using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Details : Form
    {
        private readonly int? id;
        private readonly Form1 parent;
        private const string dtFormat = "dd.MM.yyyy";
        private CultureInfo culture = new CultureInfo("en-US");
        private Color colorError = Color.LightCoral;

        public Details(int? id = null, Form1 parent = null)
        {
            this.id = id;
            this.parent = parent;
            InitializeComponent();
        }

        private void Details_Load(object sender, EventArgs e)
        {
            if (id == null)
            {
                return;
            }

            BindToForm();
        }

        private void BindToForm()
        {
            WohnungHeaderEntity header;
            WohnungDetailsEntity details;
            using (var db = new WohnungDb())
            {
                header = db.WohnungHeaders.Single(p => p.Id == id.Value);

                var detailsList = db.WohnungDetails.Where(p => p.WohnungHeaderId == header.Id).ToList();
                details = detailsList.SingleOrDefault();
            }

            ProviderTb.Text = header.Provider;
            IdTb.Text = header.WohnungId;
            GeladenTb.Text = header.Geladen.ToString(dtFormat);
            GesehenTb.Text = header.Gesehen?.ToString(dtFormat);
            GemeldetTb.Text = header.Gemeldet?.ToString(dtFormat);
            WichtigkeitTb.Text = header.Wichtigkeit.ToString();
            TriesCountTb.Text = header.LoadDetailsTries.ToString();
            SucheShortTb.Text = header.SucheShort;
            SucheDetailsTb.Text = header.SucheDetails;

            UeberschriftTb.Text = details?.Ueberschrift;
            BeschreibungTb.Text = details?.Beschreibung;
            AnschriftTb.Text = details?.Anschrift;
            BezirkTb.Text = details?.Bezirk;
            MieteWarmTb.Text = details?.MieteWarm?.ToString(culture);
            MieteKaltTb.Text = details?.MieteKalt?.ToString(culture);
            ZimmerTb.Text = details?.Zimmer?.ToString(culture);
            FlaecheTb.Text = details?.Flaeche?.ToString(culture);
            EtageTb.Text = details?.Etage?.ToString(culture);
            EtagenTb.Text = details?.Etagen?.ToString(culture);
            BindBoolToCb(WbsCb, details?.Wbs);
            BindBoolToCb(BalkonCb, details?.Balkon);
            BindBoolToCb(KellerCb, details?.Keller);
            FreiAbTb.Text = details?.FreiAb?.ToString(dtFormat);

            //todo: add new fileds here
        }

        private bool Save(bool preview)
        {
            using (var db = new WohnungDb())
            {
                var header = db.WohnungHeaders.Single(p => p.Id == id.Value);
                var details = db.WohnungDetails.SingleOrDefault(p => p.WohnungHeaderId == id.Value);
                if (details == null)
                {
                    details = new WohnungDetailsEntity
                    {
                        WohnungHeaderId = id.Value
                    };
                    db.WohnungDetails.Add(details);
                }

                var errors = 0;

                header.Gemeldet = ParseDate(GemeldetTb, ref errors);
                header.Gesehen = ParseDate(GesehenTb, ref errors);
                header.Wichtigkeit = ParseInt(WichtigkeitTb, ref errors) ?? 0;
                header.LoadDetailsTries = ParseInt(TriesCountTb, ref errors) ?? 0;
                header.SucheShort = GetStringFromTb(SucheShortTb);
                header.SucheDetails = GetStringFromTb(SucheDetailsTb);

                details.Ueberschrift = GetStringFromTb(UeberschriftTb);
                details.Beschreibung = GetStringFromTb(BeschreibungTb);
                details.Anschrift = GetStringFromTb(AnschriftTb);
                details.Bezirk = GetStringFromTb(BezirkTb);
                details.MieteWarm = ParseDecimal(MieteWarmTb, ref errors);
                details.MieteKalt = ParseDecimal(MieteKaltTb, ref errors);
                details.Zimmer = ParseInt(ZimmerTb, ref errors);
                details.Flaeche = ParseDecimal(FlaecheTb, ref errors);
                details.Etage = ParseInt(EtageTb, ref errors);
                details.Etagen = ParseInt(EtagenTb, ref errors);
                details.Wbs = GetBoolFromCb(WbsCb);
                details.Balkon = GetBoolFromCb(BalkonCb);
                details.Keller = GetBoolFromCb(KellerCb);
                details.FreiAb = ParseDate(FreiAbTb, ref errors);

                //todo: add new fileds here

                if (errors > 0)
                {
                    return false;
                }


                if (preview)
                {
                    return true;
                }

                db.SaveChanges();
            }

            //reload
            BindToForm();

            return true;
        }

        private DateTime? ParseDate(TextBox tb, ref int errors)
        {
            tb.BackColor = SystemColors.Window;
            if (string.IsNullOrEmpty(tb.Text))
            {
                return null;
            }

            var success = DateTime.TryParseExact(tb.Text, dtFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            if (success)
            {
                return result;
            }
            else
            {
                tb.BackColor = colorError;
                errors++;
                return null;
            }
        }

        private decimal? ParseDecimal(TextBox tb, ref int errors)
        {
            tb.BackColor = SystemColors.Window;
            if (string.IsNullOrEmpty(tb.Text))
            {
                return null;
            }

            var success = decimal.TryParse(tb.Text, NumberStyles.Any, culture, out var result);
            if (success)
            {
                return result;
            }
            else
            {
                tb.BackColor = colorError;
                errors++;
                return null;
            }
        }

        private int? ParseInt(TextBox tb, ref int errors)
        {
            tb.BackColor = SystemColors.Window;
            if (string.IsNullOrEmpty(tb.Text))
            {
                return null;
            }

            var success = int.TryParse(tb.Text, out var result);
            if (success)
            {
                return result;
            }
            else
            {
                tb.BackColor = colorError;
                errors++;
                return null;
            }
        }

        private void BindBoolToCb(ComboBox cb, bool? value)
        {
            if (value.HasValue)
            {
                if (value.Value)
                {
                    cb.SelectedIndex = 1;
                }
                else
                {
                    cb.SelectedIndex = 2;
                }
            }
            else
            {
                cb.SelectedIndex = 0;
            }
        }

        private bool? GetBoolFromCb(ComboBox cb)
        {
            return cb.SelectedIndex switch
            {
                0 => null,
                1 => true,
                2 => false,
                _ => throw new Exception("Unsupported value")
            };
        }

        private string GetStringFromTb(TextBox tb)
        {
            return string.IsNullOrEmpty(tb.Text)
                ? null
                : tb.Text;
        }

        private void MapBtn_Click(object sender, EventArgs e)
        {
            //url: https://www.google.de/maps/place/Blumberger+Damm+204+12679+Berlin
            var ansachrifft = AnschriftTb.Text;
            if (string.IsNullOrEmpty(ansachrifft))
            {
                MessageBox.Show("Адрес не загружен");
                return;
            }
            var parts = ansachrifft.Replace(",", " ").Replace("  ", " ").Split(' ');
            var query = string.Join("+", parts);
            var url = "https://www.google.de/maps/place/" + query;

            Form1.OpenInBrowser(url);
        }

        private void OpenProviderBtn_Click(object sender, EventArgs e)
        {
            var provider = ProviderTb.Text;
            var wohnungId = IdTb.Text;
            parent.OpenWohungInBrowser(provider, wohnungId);
        }

        private void ReloadBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Перезагрузить карточку с сайта ?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            panel1.Enabled = false;
            panel2.Enabled = false;
            Cursor = Cursors.WaitCursor;
            backgroundWorker1.RunWorkerAsync();
        }

        private void DoSave(bool ask)
        {
            if (!Save(true))
            {
                return;
            }

            if (ask)
            {
                if (MessageBox.Show("Сохранить изменения ?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
            }

            Save(false);
            parent.RefreshGrid();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            DoSave(true);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить карточку ?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            using (var db = new WohnungDb())
            {
                var details = db.WohnungDetails.Where(p => p.WohnungHeaderId == id.Value).ToList();
                foreach (var item in details)
                {
                    db.WohnungDetails.Remove(item);
                }
                var header = db.WohnungHeaders.Single(p => p.Id == id.Value);
                db.WohnungHeaders.Remove(header);
                db.SaveChanges();
            }

            parent.RefreshGrid();
            Close();
        }


        private bool reloadResult;
        private Exception reloadException;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            reloadResult = false;
            reloadException = null;
            var provider = ProviderTb.Text;
            var wohnungId = IdTb.Text;
            try
            {
                reloadResult = parent.director.TryReloadDetails(provider, wohnungId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                reloadException = ex;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panel1.Enabled = true;
            panel2.Enabled = true;
            Cursor = Cursors.Arrow;

            if (reloadException != null)
            {
                MessageBox.Show(reloadException.ToString());
            }

            if (!reloadResult)
            {
                MessageBox.Show("Ошибка загрузки данных");
                return;
            }

            BindToForm();
            parent.RefreshGrid();

            MessageBox.Show("Карточка успешно загружена и сохранена");
        }

        private void meldenBtn_Click(object sender, EventArgs e)
        {
            GemeldetTb.Text = DateTime.Today.ToString(dtFormat);
            DoSave(false);
        }
    }
}
