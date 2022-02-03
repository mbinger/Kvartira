using Common;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class RuLog : Form, ILog
    {
        public RuLog()
        {
            InitializeComponent();
            richTextBox1.HideSelection = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        public void Write(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                Action safeWrite = delegate { Write(text); };
                richTextBox1.Invoke(safeWrite);
            }
            else
            {
                richTextBox1.AppendText(text + "\n");
            }
        }

        private void RuLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
