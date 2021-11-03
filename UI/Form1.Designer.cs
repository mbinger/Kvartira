
namespace UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxRating = new System.Windows.Forms.ToolStripTextBox();
            this.StatusToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.queriesCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.nextQueryLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VermieterColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WohnungColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImportanceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GesehenColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.poolingfTimer = new System.Windows.Forms.Timer(this.components);
            this.nextQueryTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxRating,
            this.StatusToolStripComboBox,
            this.toolStripButtonRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(119, 22);
            this.toolStripLabel1.Text = "Показывать рейтинг";
            // 
            // toolStripTextBoxRating
            // 
            this.toolStripTextBoxRating.Name = "toolStripTextBoxRating";
            this.toolStripTextBoxRating.Size = new System.Drawing.Size(50, 25);
            this.toolStripTextBoxRating.Text = "1";
            // 
            // StatusToolStripComboBox
            // 
            this.StatusToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatusToolStripComboBox.Items.AddRange(new object[] {
            "только новые",
            "все"});
            this.StatusToolStripComboBox.Name = "StatusToolStripComboBox";
            this.StatusToolStripComboBox.Size = new System.Drawing.Size(170, 25);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(65, 22);
            this.toolStripButtonRefresh.Text = "Обновить";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.queriesCountLabel,
            this.toolStripStatusLabel2,
            this.nextQueryLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(66, 17);
            this.toolStripStatusLabel1.Text = "Запросов: ";
            // 
            // queriesCountLabel
            // 
            this.queriesCountLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.queriesCountLabel.Name = "queriesCountLabel";
            this.queriesCountLabel.Size = new System.Drawing.Size(14, 17);
            this.queriesCountLabel.Text = "0";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(150, 17);
            this.toolStripStatusLabel2.Text = "Следующий запрос через";
            // 
            // nextQueryLabel
            // 
            this.nextQueryLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.nextQueryLabel.Name = "nextQueryLabel";
            this.nextQueryLabel.Size = new System.Drawing.Size(38, 17);
            this.nextQueryLabel.Text = "00:00";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdColumn,
            this.VermieterColumn,
            this.WohnungColumn,
            this.ImportanceColumn,
            this.GesehenColumn,
            this.LinkColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(800, 403);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // IdColumn
            // 
            this.IdColumn.HeaderText = "Id";
            this.IdColumn.Name = "IdColumn";
            this.IdColumn.ReadOnly = true;
            this.IdColumn.Visible = false;
            // 
            // VermieterColumn
            // 
            this.VermieterColumn.HeaderText = "Арендодатель";
            this.VermieterColumn.Name = "VermieterColumn";
            this.VermieterColumn.ReadOnly = true;
            // 
            // WohnungColumn
            // 
            this.WohnungColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.WohnungColumn.HeaderText = "Номер объекта";
            this.WohnungColumn.Name = "WohnungColumn";
            this.WohnungColumn.ReadOnly = true;
            // 
            // ImportanceColumn
            // 
            this.ImportanceColumn.HeaderText = "Рейтинг";
            this.ImportanceColumn.Name = "ImportanceColumn";
            this.ImportanceColumn.ReadOnly = true;
            // 
            // GesehenColumn
            // 
            this.GesehenColumn.HeaderText = "Просмотрено";
            this.GesehenColumn.Name = "GesehenColumn";
            this.GesehenColumn.ReadOnly = true;
            // 
            // LinkColumn
            // 
            this.LinkColumn.HeaderText = "Ссылка";
            this.LinkColumn.Name = "LinkColumn";
            this.LinkColumn.ReadOnly = true;
            // 
            // poolingfTimer
            // 
            this.poolingfTimer.Tick += new System.EventHandler(this.poolingfTimer_Tick);
            // 
            // nextQueryTimer
            // 
            this.nextQueryTimer.Interval = 1000;
            this.nextQueryTimer.Tick += new System.EventHandler(this.nextQueryTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Квартиры";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxRating;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripComboBox StatusToolStripComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VermieterColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WohnungColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImportanceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn GesehenColumn;
        private System.Windows.Forms.DataGridViewLinkColumn LinkColumn;
        private System.Windows.Forms.Timer poolingfTimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel queriesCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel nextQueryLabel;
        private System.Windows.Forms.Timer nextQueryTimer;
    }
}

