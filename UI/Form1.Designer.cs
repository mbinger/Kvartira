
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
            this.wbsComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.downloadNowButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.queriesCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.nextQueryText = new System.Windows.Forms.ToolStripStatusLabel();
            this.nextQueryValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadingTimerText = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadingTimerValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WohnungColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VermieterColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZimmerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FlaecheColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PriceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WbsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BezirkColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HeaderComun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImportanceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GesehenColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.poolingfTimer = new System.Windows.Forms.Timer(this.components);
            this.nextQueryTimer = new System.Windows.Forms.Timer(this.components);
            this.loadingTimer = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonMarkAllAsRead = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxRating,
            this.StatusToolStripComboBox,
            this.wbsComboBox,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh,
            this.downloadNowButton,
            this.buttonMarkAllAsRead});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1127, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(152, 25);
            this.toolStripLabel1.Text = "Показывать рейтинг";
            // 
            // toolStripTextBoxRating
            // 
            this.toolStripTextBoxRating.Name = "toolStripTextBoxRating";
            this.toolStripTextBoxRating.Size = new System.Drawing.Size(57, 28);
            this.toolStripTextBoxRating.Text = "1";
            // 
            // StatusToolStripComboBox
            // 
            this.StatusToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatusToolStripComboBox.Items.AddRange(new object[] {
            "непросмотренные",
            "все"});
            this.StatusToolStripComboBox.Name = "StatusToolStripComboBox";
            this.StatusToolStripComboBox.Size = new System.Drawing.Size(194, 28);
            // 
            // wbsComboBox
            // 
            this.wbsComboBox.Items.AddRange(new object[] {
            "без WBS",
            "с и без WBS"});
            this.wbsComboBox.Name = "wbsComboBox";
            this.wbsComboBox.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(82, 25);
            this.toolStripButtonRefresh.Text = "Обновить";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // downloadNowButton
            // 
            this.downloadNowButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.downloadNowButton.Image = ((System.Drawing.Image)(resources.GetObject("downloadNowButton.Image")));
            this.downloadNowButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.downloadNowButton.Name = "downloadNowButton";
            this.downloadNowButton.Size = new System.Drawing.Size(171, 25);
            this.downloadNowButton.Text = "Старт загрузки данных";
            this.downloadNowButton.Click += new System.EventHandler(this.downloadNowButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.queriesCountLabel,
            this.nextQueryText,
            this.nextQueryValue,
            this.loadingTimerText,
            this.loadingTimerValue});
            this.statusStrip1.Location = new System.Drawing.Point(0, 544);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1127, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(83, 20);
            this.toolStripStatusLabel1.Text = "Запросов: ";
            // 
            // queriesCountLabel
            // 
            this.queriesCountLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.queriesCountLabel.Name = "queriesCountLabel";
            this.queriesCountLabel.Size = new System.Drawing.Size(18, 20);
            this.queriesCountLabel.Text = "0";
            // 
            // nextQueryText
            // 
            this.nextQueryText.Name = "nextQueryText";
            this.nextQueryText.Size = new System.Drawing.Size(188, 20);
            this.nextQueryText.Text = "Следующий запрос через";
            // 
            // nextQueryValue
            // 
            this.nextQueryValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.nextQueryValue.Name = "nextQueryValue";
            this.nextQueryValue.Size = new System.Drawing.Size(49, 20);
            this.nextQueryValue.Text = "00:00";
            // 
            // loadingTimerText
            // 
            this.loadingTimerText.Name = "loadingTimerText";
            this.loadingTimerText.Size = new System.Drawing.Size(69, 20);
            this.loadingTimerText.Text = "Загрузка";
            this.loadingTimerText.Visible = false;
            // 
            // loadingTimerValue
            // 
            this.loadingTimerValue.Font = new System.Drawing.Font("Segoe UI", 8.765218F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.loadingTimerValue.Name = "loadingTimerValue";
            this.loadingTimerValue.Size = new System.Drawing.Size(49, 20);
            this.loadingTimerValue.Text = "00:00";
            this.loadingTimerValue.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdColumn,
            this.WohnungColumn,
            this.VermieterColumn,
            this.ZimmerColumn,
            this.FlaecheColumn,
            this.PriceColumn,
            this.WbsColumn,
            this.BezirkColumn,
            this.AddressColumn,
            this.HeaderComun,
            this.ImportanceColumn,
            this.GesehenColumn,
            this.LinkColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 49;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(1127, 516);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // IdColumn
            // 
            this.IdColumn.HeaderText = "Id";
            this.IdColumn.MinimumWidth = 6;
            this.IdColumn.Name = "IdColumn";
            this.IdColumn.ReadOnly = true;
            this.IdColumn.Visible = false;
            this.IdColumn.Width = 120;
            // 
            // WohnungColumn
            // 
            this.WohnungColumn.HeaderText = "Номер объекта";
            this.WohnungColumn.MinimumWidth = 6;
            this.WohnungColumn.Name = "WohnungColumn";
            this.WohnungColumn.ReadOnly = true;
            this.WohnungColumn.Visible = false;
            this.WohnungColumn.Width = 383;
            // 
            // VermieterColumn
            // 
            this.VermieterColumn.HeaderText = "Арендодатель";
            this.VermieterColumn.MinimumWidth = 6;
            this.VermieterColumn.Name = "VermieterColumn";
            this.VermieterColumn.ReadOnly = true;
            this.VermieterColumn.Width = 120;
            // 
            // ZimmerColumn
            // 
            this.ZimmerColumn.HeaderText = "Комнат";
            this.ZimmerColumn.MinimumWidth = 6;
            this.ZimmerColumn.Name = "ZimmerColumn";
            this.ZimmerColumn.ReadOnly = true;
            this.ZimmerColumn.Width = 80;
            // 
            // FlaecheColumn
            // 
            this.FlaecheColumn.HeaderText = "Площадь";
            this.FlaecheColumn.MinimumWidth = 6;
            this.FlaecheColumn.Name = "FlaecheColumn";
            this.FlaecheColumn.ReadOnly = true;
            this.FlaecheColumn.Width = 80;
            // 
            // PriceColumn
            // 
            this.PriceColumn.HeaderText = "Цена";
            this.PriceColumn.MinimumWidth = 6;
            this.PriceColumn.Name = "PriceColumn";
            this.PriceColumn.ReadOnly = true;
            this.PriceColumn.Width = 80;
            // 
            // WbsColumn
            // 
            this.WbsColumn.HeaderText = "WBS";
            this.WbsColumn.MinimumWidth = 6;
            this.WbsColumn.Name = "WbsColumn";
            this.WbsColumn.ReadOnly = true;
            this.WbsColumn.Width = 60;
            // 
            // BezirkColumn
            // 
            this.BezirkColumn.HeaderText = "Район";
            this.BezirkColumn.MinimumWidth = 6;
            this.BezirkColumn.Name = "BezirkColumn";
            this.BezirkColumn.ReadOnly = true;
            this.BezirkColumn.Width = 120;
            // 
            // AddressColumn
            // 
            this.AddressColumn.HeaderText = "Адрес";
            this.AddressColumn.MinimumWidth = 6;
            this.AddressColumn.Name = "AddressColumn";
            this.AddressColumn.ReadOnly = true;
            this.AddressColumn.Width = 200;
            // 
            // HeaderComun
            // 
            this.HeaderComun.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.HeaderComun.HeaderText = "Описание";
            this.HeaderComun.MinimumWidth = 6;
            this.HeaderComun.Name = "HeaderComun";
            this.HeaderComun.ReadOnly = true;
            // 
            // ImportanceColumn
            // 
            this.ImportanceColumn.HeaderText = "Рейтинг";
            this.ImportanceColumn.MinimumWidth = 6;
            this.ImportanceColumn.Name = "ImportanceColumn";
            this.ImportanceColumn.ReadOnly = true;
            this.ImportanceColumn.Width = 80;
            // 
            // GesehenColumn
            // 
            this.GesehenColumn.HeaderText = "Просмотрено";
            this.GesehenColumn.MinimumWidth = 6;
            this.GesehenColumn.Name = "GesehenColumn";
            this.GesehenColumn.ReadOnly = true;
            this.GesehenColumn.Width = 120;
            // 
            // LinkColumn
            // 
            this.LinkColumn.HeaderText = "Ссылка";
            this.LinkColumn.MinimumWidth = 6;
            this.LinkColumn.Name = "LinkColumn";
            this.LinkColumn.ReadOnly = true;
            this.LinkColumn.Width = 80;
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
            // loadingTimer
            // 
            this.loadingTimer.Interval = 1000;
            this.loadingTimer.Tick += new System.EventHandler(this.loadingTimer_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // buttonMarkAllAsRead
            // 
            this.buttonMarkAllAsRead.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonMarkAllAsRead.Image = ((System.Drawing.Image)(resources.GetObject("buttonMarkAllAsRead.Image")));
            this.buttonMarkAllAsRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMarkAllAsRead.Name = "buttonMarkAllAsRead";
            this.buttonMarkAllAsRead.Size = new System.Drawing.Size(252, 25);
            this.buttonMarkAllAsRead.Text = "Пометить все как просмотренные";
            this.buttonMarkAllAsRead.Click += new System.EventHandler(this.buttonMarkAllAsRead_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 570);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Квартиры";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
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
        private System.Windows.Forms.Timer poolingfTimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel queriesCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel nextQueryText;
        private System.Windows.Forms.ToolStripStatusLabel nextQueryValue;
        private System.Windows.Forms.Timer nextQueryTimer;
        private System.Windows.Forms.ToolStripButton downloadNowButton;
        private System.Windows.Forms.ToolStripStatusLabel loadingTimerText;
        private System.Windows.Forms.ToolStripStatusLabel loadingTimerValue;
        private System.Windows.Forms.Timer loadingTimer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WohnungColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VermieterColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZimmerColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FlaecheColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PriceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WbsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BezirkColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddressColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn HeaderComun;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImportanceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn GesehenColumn;
        private System.Windows.Forms.DataGridViewLinkColumn LinkColumn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox wbsComboBox;
        private System.Windows.Forms.ToolStripButton buttonMarkAllAsRead;
    }
}

