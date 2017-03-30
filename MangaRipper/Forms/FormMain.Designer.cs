namespace MangaRipper.Forms
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnGetChapter = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.dgvQueueChapter = new System.Windows.Forms.DataGridView();
            this.ColChapterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColChapterStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColChapterUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Formats = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnChangeSaveTo = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtPercent = new System.Windows.Forms.TextBox();
            this.dgvSupportedSites = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvChapter = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveDestinationDirectoryBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.cbTitleUrl = new System.Windows.Forms.ComboBox();
            this.btnAddBookmark = new System.Windows.Forms.Button();
            this.btnRemoveBookmark = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxForPrefix = new System.Windows.Forms.CheckBox();
            this.lbDefaultDestination = new System.Windows.Forms.Label();
            this.lbSeriesDestination = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.cbSaveFolder = new System.Windows.Forms.CheckBox();
            this.cbSaveCbz = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.locationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rdDefaultDestination = new System.Windows.Forms.RadioButton();
            this.rdSeriesDestination = new System.Windows.Forms.RadioButton();
            this.lbDestination = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueueChapter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportedSites)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChapter)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGetChapter
            // 
            this.btnGetChapter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetChapter.Location = new System.Drawing.Point(876, 27);
            this.btnGetChapter.Name = "btnGetChapter";
            this.btnGetChapter.Size = new System.Drawing.Size(116, 23);
            this.btnGetChapter.TabIndex = 5;
            this.btnGetChapter.Text = "Get Chapters";
            this.toolTip1.SetToolTip(this.btnGetChapter, "Get Chapters from Inputed Url");
            this.btnGetChapter.UseVisualStyleBackColor = true;
            this.btnGetChapter.Click += new System.EventHandler(this.btnGetChapter_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Location = new System.Drawing.Point(734, 551);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(147, 23);
            this.btnDownload.TabIndex = 18;
            this.btnDownload.Text = "Start Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Webdings", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnAdd.Location = new System.Drawing.Point(443, 55);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(62, 33);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "4";
            this.toolTip1.SetToolTip(this.btnAdd, "Add selected chapters");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Font = new System.Drawing.Font("Webdings", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnAddAll.Location = new System.Drawing.Point(443, 94);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(62, 32);
            this.btnAddAll.TabIndex = 9;
            this.btnAddAll.Text = "8";
            this.toolTip1.SetToolTip(this.btnAddAll, "Add all chapters");
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(512, 551);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(105, 23);
            this.btnRemove.TabIndex = 16;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveAll.Location = new System.Drawing.Point(623, 551);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(105, 23);
            this.btnRemoveAll.TabIndex = 17;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // dgvQueueChapter
            // 
            this.dgvQueueChapter.AllowUserToAddRows = false;
            this.dgvQueueChapter.AllowUserToDeleteRows = false;
            this.dgvQueueChapter.AllowUserToResizeRows = false;
            this.dgvQueueChapter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvQueueChapter.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvQueueChapter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvQueueChapter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQueueChapter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColChapterName,
            this.ColChapterStatus,
            this.ColChapterUrl,
            this.Formats});
            this.dgvQueueChapter.Location = new System.Drawing.Point(511, 55);
            this.dgvQueueChapter.Name = "dgvQueueChapter";
            this.dgvQueueChapter.ReadOnly = true;
            this.dgvQueueChapter.RowHeadersVisible = false;
            this.dgvQueueChapter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQueueChapter.Size = new System.Drawing.Size(481, 490);
            this.dgvQueueChapter.TabIndex = 10;
            // 
            // ColChapterName
            // 
            this.ColChapterName.DataPropertyName = "Name";
            this.ColChapterName.HeaderText = "Chapter Name";
            this.ColChapterName.Name = "ColChapterName";
            this.ColChapterName.ReadOnly = true;
            this.ColChapterName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColChapterName.Width = 200;
            // 
            // ColChapterStatus
            // 
            this.ColChapterStatus.DataPropertyName = "Percent";
            this.ColChapterStatus.HeaderText = "%";
            this.ColChapterStatus.Name = "ColChapterStatus";
            this.ColChapterStatus.ReadOnly = true;
            this.ColChapterStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColChapterStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColChapterStatus.Width = 35;
            // 
            // ColChapterUrl
            // 
            this.ColChapterUrl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColChapterUrl.DataPropertyName = "SaveToFolder";
            this.ColChapterUrl.HeaderText = "Save To";
            this.ColChapterUrl.Name = "ColChapterUrl";
            this.ColChapterUrl.ReadOnly = true;
            this.ColChapterUrl.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColChapterUrl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Formats
            // 
            this.Formats.DataPropertyName = "PropFormats";
            this.Formats.HeaderText = "Formats";
            this.Formats.Name = "Formats";
            this.Formats.ReadOnly = true;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(887, 551);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(105, 23);
            this.btnStop.TabIndex = 19;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnChangeSaveTo
            // 
            this.btnChangeSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeSaveTo.Location = new System.Drawing.Point(312, 288);
            this.btnChangeSaveTo.Name = "btnChangeSaveTo";
            this.btnChangeSaveTo.Size = new System.Drawing.Size(30, 23);
            this.btnChangeSaveTo.TabIndex = 13;
            this.btnChangeSaveTo.Text = "...";
            this.toolTip1.SetToolTip(this.btnChangeSaveTo, "Change Folder");
            this.btnChangeSaveTo.UseVisualStyleBackColor = true;
            this.btnChangeSaveTo.Click += new System.EventHandler(this.btnChangeSaveTo_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenFolder.Location = new System.Drawing.Point(348, 288);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(89, 23);
            this.btnOpenFolder.TabIndex = 14;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(12, 31);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(22, 13);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Url";
            // 
            // txtPercent
            // 
            this.txtPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPercent.Enabled = false;
            this.txtPercent.Location = new System.Drawing.Point(835, 28);
            this.txtPercent.Name = "txtPercent";
            this.txtPercent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtPercent.Size = new System.Drawing.Size(35, 22);
            this.txtPercent.TabIndex = 4;
            // 
            // dgvSupportedSites
            // 
            this.dgvSupportedSites.AllowUserToAddRows = false;
            this.dgvSupportedSites.AllowUserToDeleteRows = false;
            this.dgvSupportedSites.AllowUserToResizeRows = false;
            this.dgvSupportedSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvSupportedSites.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSupportedSites.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSupportedSites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSupportedSites.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.Column1});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSupportedSites.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSupportedSites.Location = new System.Drawing.Point(12, 448);
            this.dgvSupportedSites.MultiSelect = false;
            this.dgvSupportedSites.Name = "dgvSupportedSites";
            this.dgvSupportedSites.ReadOnly = true;
            this.dgvSupportedSites.RowHeadersVisible = false;
            this.dgvSupportedSites.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSupportedSites.Size = new System.Drawing.Size(425, 126);
            this.dgvSupportedSites.TabIndex = 15;
            this.dgvSupportedSites.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSupportedSites_CellContentClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Url";
            this.dataGridViewTextBoxColumn3.HeaderText = "Address";
            this.dataGridViewTextBoxColumn3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.dataGridViewTextBoxColumn3.LinkColor = System.Drawing.Color.Blue;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewTextBoxColumn3.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Language";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 90;
            // 
            // dgvChapter
            // 
            this.dgvChapter.AllowUserToAddRows = false;
            this.dgvChapter.AllowUserToDeleteRows = false;
            this.dgvChapter.AllowUserToResizeRows = false;
            this.dgvChapter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvChapter.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvChapter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvChapter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChapter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4});
            this.dgvChapter.Location = new System.Drawing.Point(12, 55);
            this.dgvChapter.Name = "dgvChapter";
            this.dgvChapter.ReadOnly = true;
            this.dgvChapter.RowHeadersVisible = false;
            this.dgvChapter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChapter.Size = new System.Drawing.Size(425, 227);
            this.dgvChapter.TabIndex = 6;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn4.HeaderText = "Chapter Name";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cbTitleUrl
            // 
            this.cbTitleUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTitleUrl.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbTitleUrl.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbTitleUrl.Location = new System.Drawing.Point(40, 28);
            this.cbTitleUrl.Name = "cbTitleUrl";
            this.cbTitleUrl.Size = new System.Drawing.Size(729, 21);
            this.cbTitleUrl.TabIndex = 1;
            // 
            // btnAddBookmark
            // 
            this.btnAddBookmark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBookmark.Location = new System.Drawing.Point(775, 27);
            this.btnAddBookmark.Name = "btnAddBookmark";
            this.btnAddBookmark.Size = new System.Drawing.Size(24, 23);
            this.btnAddBookmark.TabIndex = 2;
            this.btnAddBookmark.Text = "B";
            this.toolTip1.SetToolTip(this.btnAddBookmark, "Bookmark This Url");
            this.btnAddBookmark.UseVisualStyleBackColor = true;
            this.btnAddBookmark.Click += new System.EventHandler(this.btnAddBookmark_Click);
            // 
            // btnRemoveBookmark
            // 
            this.btnRemoveBookmark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBookmark.Location = new System.Drawing.Point(805, 27);
            this.btnRemoveBookmark.Name = "btnRemoveBookmark";
            this.btnRemoveBookmark.Size = new System.Drawing.Size(24, 23);
            this.btnRemoveBookmark.TabIndex = 3;
            this.btnRemoveBookmark.Text = "R";
            this.toolTip1.SetToolTip(this.btnRemoveBookmark, "Remove This Url From Bookmark");
            this.btnRemoveBookmark.UseVisualStyleBackColor = true;
            this.btnRemoveBookmark.Click += new System.EventHandler(this.btnRemoveBookmark_Click);
            // 
            // checkBoxForPrefix
            // 
            this.checkBoxForPrefix.AutoSize = true;
            this.checkBoxForPrefix.Location = new System.Drawing.Point(6, 18);
            this.checkBoxForPrefix.Name = "checkBoxForPrefix";
            this.checkBoxForPrefix.Size = new System.Drawing.Size(99, 17);
            this.checkBoxForPrefix.TabIndex = 0;
            this.checkBoxForPrefix.Text = "Prefix Counter";
            this.toolTip1.SetToolTip(this.checkBoxForPrefix, "Add prefix to chapters");
            this.checkBoxForPrefix.UseVisualStyleBackColor = true;
            this.checkBoxForPrefix.CheckedChanged += new System.EventHandler(this.checkBoxForPrefix_CheckedChanged);
            // 
            // lbDefaultDestination
            // 
            this.lbDefaultDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbDefaultDestination.AutoSize = true;
            this.lbDefaultDestination.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDefaultDestination.Location = new System.Drawing.Point(55, 323);
            this.lbDefaultDestination.MaximumSize = new System.Drawing.Size(385, 17);
            this.lbDefaultDestination.MinimumSize = new System.Drawing.Size(380, 17);
            this.lbDefaultDestination.Name = "lbDefaultDestination";
            this.lbDefaultDestination.Size = new System.Drawing.Size(380, 17);
            this.lbDefaultDestination.TabIndex = 34;
            this.lbDefaultDestination.Text = "Default Destination";
            this.toolTip1.SetToolTip(this.lbDefaultDestination, "Saves the chapter to the default manga folder");
            this.lbDefaultDestination.UseMnemonic = false;
            this.lbDefaultDestination.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbDefaultDestination_MouseClick);
            // 
            // lbSeriesDestination
            // 
            this.lbSeriesDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSeriesDestination.AutoSize = true;
            this.lbSeriesDestination.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSeriesDestination.Location = new System.Drawing.Point(55, 358);
            this.lbSeriesDestination.MaximumSize = new System.Drawing.Size(380, 17);
            this.lbSeriesDestination.MinimumSize = new System.Drawing.Size(380, 17);
            this.lbSeriesDestination.Name = "lbSeriesDestination";
            this.lbSeriesDestination.Size = new System.Drawing.Size(380, 17);
            this.lbSeriesDestination.TabIndex = 35;
            this.lbSeriesDestination.Text = "Series Specific Destination";
            this.toolTip1.SetToolTip(this.lbSeriesDestination, "Saves the chapter to the series\' folder");
            this.lbSeriesDestination.UseMnemonic = false;
            this.lbSeriesDestination.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbSeriesDestination_MouseClick);
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Location = new System.Drawing.Point(0, 584);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(1004, 22);
            this.txtMessage.TabIndex = 23;
            // 
            // cbSaveFolder
            // 
            this.cbSaveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSaveFolder.AutoSize = true;
            this.cbSaveFolder.Checked = true;
            this.cbSaveFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSaveFolder.Location = new System.Drawing.Point(4, 18);
            this.cbSaveFolder.Name = "cbSaveFolder";
            this.cbSaveFolder.Size = new System.Drawing.Size(59, 17);
            this.cbSaveFolder.TabIndex = 27;
            this.cbSaveFolder.Text = "Folder";
            this.cbSaveFolder.UseVisualStyleBackColor = true;
            // 
            // cbSaveCbz
            // 
            this.cbSaveCbz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSaveCbz.AutoSize = true;
            this.cbSaveCbz.Location = new System.Drawing.Point(71, 18);
            this.cbSaveCbz.Name = "cbSaveCbz";
            this.cbSaveCbz.Size = new System.Drawing.Size(46, 17);
            this.cbSaveCbz.TabIndex = 28;
            this.cbSaveCbz.Text = "CBZ";
            this.cbSaveCbz.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cbSaveFolder);
            this.groupBox1.Controls.Add(this.cbSaveCbz);
            this.groupBox1.Location = new System.Drawing.Point(12, 401);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(118, 41);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Save As";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.checkBoxForPrefix);
            this.groupBox2.Location = new System.Drawing.Point(136, 401);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(120, 41);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File manipulation";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.locationToolStripMenuItem,
            this.documentsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1004, 24);
            this.menuStrip1.TabIndex = 31;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // locationToolStripMenuItem
            // 
            this.locationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataToolStripMenuItem,
            this.logsToolStripMenuItem});
            this.locationToolStripMenuItem.Name = "locationToolStripMenuItem";
            this.locationToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.locationToolStripMenuItem.Text = "Locations";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.dataToolStripMenuItem.Text = "Data";
            this.dataToolStripMenuItem.Click += new System.EventHandler(this.dataToolStripMenuItem_Click);
            // 
            // logsToolStripMenuItem
            // 
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            this.logsToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.logsToolStripMenuItem.Text = "Logs";
            this.logsToolStripMenuItem.Click += new System.EventHandler(this.logsToolStripMenuItem_Click);
            // 
            // documentsToolStripMenuItem
            // 
            this.documentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wikiToolStripMenuItem,
            this.bugReportToolStripMenuItem});
            this.documentsToolStripMenuItem.Name = "documentsToolStripMenuItem";
            this.documentsToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.documentsToolStripMenuItem.Text = "Documents";
            // 
            // wikiToolStripMenuItem
            // 
            this.wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            this.wikiToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.wikiToolStripMenuItem.Text = "Wiki";
            this.wikiToolStripMenuItem.Click += new System.EventHandler(this.wikiToolStripMenuItem_Click);
            // 
            // bugReportToolStripMenuItem
            // 
            this.bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            this.bugReportToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.bugReportToolStripMenuItem.Text = "Bug Report";
            this.bugReportToolStripMenuItem.Click += new System.EventHandler(this.bugReportToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // rdDefaultDestination
            // 
            this.rdDefaultDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdDefaultDestination.AutoSize = true;
            this.rdDefaultDestination.Checked = true;
            this.rdDefaultDestination.Location = new System.Drawing.Point(35, 326);
            this.rdDefaultDestination.Name = "rdDefaultDestination";
            this.rdDefaultDestination.Size = new System.Drawing.Size(14, 13);
            this.rdDefaultDestination.TabIndex = 32;
            this.rdDefaultDestination.TabStop = true;
            this.rdDefaultDestination.UseVisualStyleBackColor = true;
            // 
            // rdSeriesDestination
            // 
            this.rdSeriesDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdSeriesDestination.AutoSize = true;
            this.rdSeriesDestination.Location = new System.Drawing.Point(35, 361);
            this.rdSeriesDestination.Name = "rdSeriesDestination";
            this.rdSeriesDestination.Size = new System.Drawing.Size(14, 13);
            this.rdSeriesDestination.TabIndex = 33;
            this.rdSeriesDestination.UseVisualStyleBackColor = true;
            // 
            // lbDestination
            // 
            this.lbDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbDestination.AutoSize = true;
            this.lbDestination.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestination.Location = new System.Drawing.Point(11, 291);
            this.lbDestination.Name = "lbDestination";
            this.lbDestination.Size = new System.Drawing.Size(123, 20);
            this.lbDestination.TabIndex = 26;
            this.lbDestination.Text = "Save Destination";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 606);
            this.Controls.Add(this.lbSeriesDestination);
            this.Controls.Add(this.lbDefaultDestination);
            this.Controls.Add(this.lbDestination);
            this.Controls.Add(this.rdSeriesDestination);
            this.Controls.Add(this.rdDefaultDestination);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnRemoveBookmark);
            this.Controls.Add(this.btnAddBookmark);
            this.Controls.Add(this.cbTitleUrl);
            this.Controls.Add(this.dgvSupportedSites);
            this.Controls.Add(this.txtPercent);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.btnChangeSaveTo);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnGetChapter);
            this.Controls.Add(this.dgvQueueChapter);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dgvChapter);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1020, 644);
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueueChapter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportedSites)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChapter)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetChapter;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.DataGridView dgvQueueChapter;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnChangeSaveTo;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtPercent;
        private System.Windows.Forms.DataGridView dgvSupportedSites;
        private System.Windows.Forms.DataGridView dgvChapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.FolderBrowserDialog saveDestinationDirectoryBrowser;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewLinkColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.ComboBox cbTitleUrl;
        private System.Windows.Forms.Button btnAddBookmark;
        private System.Windows.Forms.Button btnRemoveBookmark;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.CheckBox cbSaveFolder;
        private System.Windows.Forms.CheckBox cbSaveCbz;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterUrl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Formats;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxForPrefix;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem locationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wikiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bugReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.RadioButton rdDefaultDestination;
        private System.Windows.Forms.RadioButton rdSeriesDestination;
        private System.Windows.Forms.Label lbDestination;
        private System.Windows.Forms.Label lbDefaultDestination;
        private System.Windows.Forms.Label lbSeriesDestination;
    }
}