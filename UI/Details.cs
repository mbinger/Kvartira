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
        private readonly Guid? id;
        private readonly Form1 parent;
        private const string dtFormat = "dd.MM.yyyy";
        private CultureInfo culture = new CultureInfo("en-US");
        private Color colorError = Color.LightCoral;

        public Details(Guid? id = null, Form1 parent = null)
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
            WohnungEntity w;
            using (var db = new WohnungDb())
            {
                w = db.Wohnungen.Single(p => p.Id == id.Value);
            }

            ProviderTb.Text = w.Provider;
            IdTb.Text = w.WohnungId;
            GeladenTb.Text = w.Geladen.ToString(dtFormat);
            GesehenTb.Text = w.Gesehen?.ToString(dtFormat);
            GemeldetTb.Text = w.Gemeldet?.ToString(dtFormat);
            WichtigkeitTb.Text = w.Wichtigkeit.ToString();
            TriesCountTb.Text = w.LoadDetailsTries.ToString();
            SucheShortTb.Text = w.SucheShort;
            SucheDetailsTb.Text = w.SucheDetails;

            UeberschriftTb.Text = w.Ueberschrift;
            BeschreibungTb.Text = w.Beschreibung;
            AnschriftTb.Text = w.Anschrift;
            BezirkTb.Text = w.Bezirk;
            MieteWarmTb.Text = w.MieteWarm?.ToString(culture);
            MieteKaltTb.Text = w.MieteKalt?.ToString(culture);
            ZimmerTb.Text = w.Zimmer?.ToString(culture);
            FlaecheTb.Text = w.Flaeche?.ToString(culture);
            EtageTb.Text = w.Etage?.ToString(culture);
            EtagenTb.Text = w.Etagen?.ToString(culture);
            BindBoolToCb(WbsCb, w.Wbs);
            BindBoolToCb(BalkonCb, w.Balkon);
            BindBoolToCb(KellerCb, w.Keller);
            FreiAbTb.Text = w.FreiAb?.ToString(dtFormat);

            //todo: add new fileds here
        }

        private bool Save(bool preview)
        {
            using (var db = new WohnungDb())
            {
                var w = db.Wohnungen.Single(p => p.Id == id.Value);

                var errors = 0;

                w.Gemeldet = ParseDate(GemeldetTb, ref errors);
                w.Gesehen = ParseDate(GesehenTb, ref errors);
                w.Wichtigkeit = ParseInt(WichtigkeitTb, ref errors) ?? 0;
                w.LoadDetailsTries = ParseInt(TriesCountTb, ref errors) ?? 0;
                w.SucheShort = GetStringFromTb(SucheShortTb);
                w.SucheDetails = GetStringFromTb(SucheDetailsTb);

                w.Ueberschrift = GetStringFromTb(UeberschriftTb);
                w.Beschreibung = GetStringFromTb(BeschreibungTb);
                w.Anschrift = GetStringFromTb(AnschriftTb);
                w.Bezirk = GetStringFromTb(BezirkTb);
                w.MieteWarm = ParseDecimal(MieteWarmTb, ref errors);
                w.MieteKalt = ParseDecimal(MieteKaltTb, ref errors);
                w.Zimmer = ParseInt(ZimmerTb, ref errors);
                w.Flaeche = ParseDecimal(FlaecheTb, ref errors);
                w.Etage = ParseInt(EtageTb, ref errors);
                w.Etagen = ParseInt(EtagenTb, ref errors);
                w.Wbs = GetBoolFromCb(WbsCb);
                w.Balkon = GetBoolFromCb(BalkonCb);
                w.Keller = GetBoolFromCb(KellerCb);
                w.FreiAb = ParseDate(FreiAbTb, ref errors);

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
                var wList = db.Wohnungen.Where(p => p.Id == id.Value).ToList();
                foreach (var item in wList)
                {
                    db.Wohnungen.Remove(item);
                }
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
