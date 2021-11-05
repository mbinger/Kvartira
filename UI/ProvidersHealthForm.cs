using Common;
using Providers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    public partial class ProvidersHealthForm : Form
    {
        private readonly Director director;

        public ProvidersHealthForm(Director director)
        {
            this.director = director;
            InitializeComponent();
        }

        private void ProvidersHealthForm_Load(object sender, EventArgs e)
        {
            var data = director.GetRating();
            dataGridView1.Rows.Clear();

            string format(DateTime? dt)
            {
                if (dt == null)
                {
                    return "";
                }

                return dt.Value.ToString("dd.MM.yyyy HH:mm");
            }

            foreach (var item in data)
            {
                if (item.Entry == null)
                {
                    continue;
                }
                dataGridView1.Rows.Add(item.Entry.ProviderName,
                    format(item.Entry.LastUpdate),
                    format(item.Entry.IdsLoaded),
                    format(item.Entry.NewIdsLoaded),
                    format(item.Entry.DetailsLoaded),
                    format(item.Entry.AllDetailsComlete),
                    item.Reason,
                    ((int)item.Color).ToString(),
                    "");
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var colorStr = (string)row.Cells[Column8.Index].Value;
                var colorInt = int.Parse(colorStr);
                var color = (ProviderHealthColor)colorInt;

                var targetCell = row.Cells[Column7.Index];
                if (color == ProviderHealthColor.Gray)
                {
                    targetCell.Style.BackColor = SystemColors.Control;
                }
                else if (color == ProviderHealthColor.Green)
                {
                    targetCell.Style.BackColor = Color.DarkGreen;
                }
                else if (color == ProviderHealthColor.Yellow)
                {
                    targetCell.Style.BackColor = Color.Yellow;
                }
                else if (color == ProviderHealthColor.Red)
                {
                    targetCell.Style.BackColor = Color.Red;
                }
            }
        }
    }
}
