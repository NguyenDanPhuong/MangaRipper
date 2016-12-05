namespace MangaRipper
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
            this.btnStop = new System.Windows.Forms.Button();
            this.btnChangeSaveTo = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lbSaveTo = new System.Windows.Forms.Label();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtPercent = new System.Windows.Forms.TextBox();
            this.dgvSupportedSites = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnHowToUse = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.dgvChapter = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cbTitleUrl = new System.Windows.Forms.ComboBox();
            this.btnAddBookmark = new System.Windows.Forms.Button();
            this.btnRemoveBookmark = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddPrefixCounter = new System.Windows.Forms.Button();
            this.txtSaveTo = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSaveFolder = new System.Windows.Forms.CheckBox();
            this.cbSaveCbz = new System.Windows.Forms.CheckBox();
            this.ColChapterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColChapterStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColChapterUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueueChapter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportedSites)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChapter)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetChapter
            // 
            this.btnGetChapter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetChapter.Location = new System.Drawing.Point(876, 10);
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
            this.btnDownload.Location = new System.Drawing.Point(665, 551);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(216, 23);
            this.btnDownload.TabIndex = 18;
            this.btnDownload.Text = "Start Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(153, 288);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(143, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add Selected To Queue";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddAll.Location = new System.Drawing.Point(302, 288);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(135, 23);
            this.btnAddAll.TabIndex = 9;
            this.btnAddAll.Text = "Add All To Queue";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(443, 551);
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
            this.btnRemoveAll.Location = new System.Drawing.Point(554, 551);
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
            this.ColChapterUrl});
            this.dgvQueueChapter.Location = new System.Drawing.Point(443, 38);
            this.dgvQueueChapter.Name = "dgvQueueChapter";
            this.dgvQueueChapter.ReadOnly = true;
            this.dgvQueueChapter.RowHeadersVisible = false;
            this.dgvQueueChapter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQueueChapter.Size = new System.Drawing.Size(549, 507);
            this.dgvQueueChapter.TabIndex = 10;
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
            this.btnChangeSaveTo.Location = new System.Drawing.Point(312, 317);
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
            this.btnOpenFolder.Location = new System.Drawing.Point(348, 317);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(89, 23);
            this.btnOpenFolder.TabIndex = 14;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // lbSaveTo
            // 
            this.lbSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSaveTo.AutoSize = true;
            this.lbSaveTo.Location = new System.Drawing.Point(12, 321);
            this.lbSaveTo.Name = "lbSaveTo";
            this.lbSaveTo.Size = new System.Drawing.Size(45, 13);
            this.lbSaveTo.TabIndex = 11;
            this.lbSaveTo.Text = "Save To";
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(12, 14);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(22, 13);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Url";
            // 
            // txtPercent
            // 
            this.txtPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPercent.Enabled = false;
            this.txtPercent.Location = new System.Drawing.Point(835, 11);
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
            this.dgvSupportedSites.Location = new System.Drawing.Point(12, 375);
            this.dgvSupportedSites.MultiSelect = false;
            this.dgvSupportedSites.Name = "dgvSupportedSites";
            this.dgvSupportedSites.ReadOnly = true;
            this.dgvSupportedSites.RowHeadersVisible = false;
            this.dgvSupportedSites.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSupportedSites.Size = new System.Drawing.Size(425, 170);
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
            // btnHowToUse
            // 
            this.btnHowToUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHowToUse.Location = new System.Drawing.Point(15, 551);
            this.btnHowToUse.Name = "btnHowToUse";
            this.btnHowToUse.Size = new System.Drawing.Size(273, 23);
            this.btnHowToUse.TabIndex = 21;
            this.btnHowToUse.Text = "How To Use && F.A.Q.";
            this.btnHowToUse.UseVisualStyleBackColor = true;
            this.btnHowToUse.Click += new System.EventHandler(this.btnHowToUse_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAbout.Location = new System.Drawing.Point(294, 551);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(143, 23);
            this.btnAbout.TabIndex = 22;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
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
            this.dgvChapter.Location = new System.Drawing.Point(12, 38);
            this.dgvChapter.Name = "dgvChapter";
            this.dgvChapter.ReadOnly = true;
            this.dgvChapter.RowHeadersVisible = false;
            this.dgvChapter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChapter.Size = new System.Drawing.Size(425, 244);
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
            this.cbTitleUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MangaRipper.Properties.Settings.Default, "Url", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbTitleUrl.Location = new System.Drawing.Point(40, 11);
            this.cbTitleUrl.Name = "cbTitleUrl";
            this.cbTitleUrl.Size = new System.Drawing.Size(729, 21);
            this.cbTitleUrl.TabIndex = 1;
            this.cbTitleUrl.Text = global::MangaRipper.Properties.Settings.Default.Url;
            // 
            // btnAddBookmark
            // 
            this.btnAddBookmark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBookmark.Location = new System.Drawing.Point(775, 10);
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
            this.btnRemoveBookmark.Location = new System.Drawing.Point(805, 10);
            this.btnRemoveBookmark.Name = "btnRemoveBookmark";
            this.btnRemoveBookmark.Size = new System.Drawing.Size(24, 23);
            this.btnRemoveBookmark.TabIndex = 3;
            this.btnRemoveBookmark.Text = "R";
            this.toolTip1.SetToolTip(this.btnRemoveBookmark, "Remove This Url From Bookmark");
            this.btnRemoveBookmark.UseVisualStyleBackColor = true;
            this.btnRemoveBookmark.Click += new System.EventHandler(this.btnRemoveBookmark_Click);
            // 
            // btnAddPrefixCounter
            // 
            this.btnAddPrefixCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPrefixCounter.Location = new System.Drawing.Point(12, 288);
            this.btnAddPrefixCounter.Name = "btnAddPrefixCounter";
            this.btnAddPrefixCounter.Size = new System.Drawing.Size(135, 23);
            this.btnAddPrefixCounter.TabIndex = 7;
            this.btnAddPrefixCounter.Text = "Add Prefix Counter";
            this.toolTip1.SetToolTip(this.btnAddPrefixCounter, "Add Prefix Counter to Chapter Name");
            this.btnAddPrefixCounter.UseVisualStyleBackColor = true;
            this.btnAddPrefixCounter.Click += new System.EventHandler(this.btnAddPrefixCounter_Click);
            // 
            // txtSaveTo
            // 
            this.txtSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSaveTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MangaRipper.Properties.Settings.Default, "SaveTo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtSaveTo.Location = new System.Drawing.Point(66, 318);
            this.txtSaveTo.Name = "txtSaveTo";
            this.txtSaveTo.ReadOnly = true;
            this.txtSaveTo.Size = new System.Drawing.Size(240, 22);
            this.txtSaveTo.TabIndex = 12;
            this.txtSaveTo.Text = global::MangaRipper.Properties.Settings.Default.SaveTo;
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
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 350);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Save As";
            // 
            // cbSaveFolder
            // 
            this.cbSaveFolder.AutoSize = true;
            this.cbSaveFolder.Checked = true;
            this.cbSaveFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSaveFolder.Enabled = false;
            this.cbSaveFolder.Location = new System.Drawing.Point(66, 349);
            this.cbSaveFolder.Name = "cbSaveFolder";
            this.cbSaveFolder.Size = new System.Drawing.Size(59, 17);
            this.cbSaveFolder.TabIndex = 27;
            this.cbSaveFolder.Text = "Folder";
            this.cbSaveFolder.UseVisualStyleBackColor = true;
            // 
            // cbSaveCbz
            // 
            this.cbSaveCbz.AutoSize = true;
            this.cbSaveCbz.Location = new System.Drawing.Point(131, 349);
            this.cbSaveCbz.Name = "cbSaveCbz";
            this.cbSaveCbz.Size = new System.Drawing.Size(46, 17);
            this.cbSaveCbz.TabIndex = 28;
            this.cbSaveCbz.Text = "CBZ";
            this.cbSaveCbz.UseVisualStyleBackColor = true;
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
            this.ColChapterUrl.DataPropertyName = "Url";
            this.ColChapterUrl.HeaderText = "Address";
            this.ColChapterUrl.Name = "ColChapterUrl";
            this.ColChapterUrl.ReadOnly = true;
            this.ColChapterUrl.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColChapterUrl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 606);
            this.Controls.Add(this.cbSaveCbz);
            this.Controls.Add(this.cbSaveFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnAddPrefixCounter);
            this.Controls.Add(this.btnRemoveBookmark);
            this.Controls.Add(this.btnAddBookmark);
            this.Controls.Add(this.cbTitleUrl);
            this.Controls.Add(this.dgvChapter);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnHowToUse);
            this.Controls.Add(this.dgvSupportedSites);
            this.Controls.Add(this.txtPercent);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.txtSaveTo);
            this.Controls.Add(this.lbSaveTo);
            this.Controls.Add(this.btnChangeSaveTo);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.dgvQueueChapter);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnGetChapter);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1020, 644);
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueueChapter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportedSites)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChapter)).EndInit();
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
        private System.Windows.Forms.TextBox txtSaveTo;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnChangeSaveTo;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Label lbSaveTo;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtPercent;
        private System.Windows.Forms.DataGridView dgvSupportedSites;
        private System.Windows.Forms.Button btnHowToUse;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.DataGridView dgvChapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewLinkColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.ComboBox cbTitleUrl;
        private System.Windows.Forms.Button btnAddBookmark;
        private System.Windows.Forms.Button btnRemoveBookmark;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnAddPrefixCounter;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbSaveFolder;
        private System.Windows.Forms.CheckBox cbSaveCbz;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChapterUrl;
    }
}