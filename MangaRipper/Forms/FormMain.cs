using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core.DataTypes;
using MangaRipper.Core.Models;
using MangaRipper.Core.Providers;
using MangaRipper.Helpers;
using MangaRipper.Presenters;
using NLog;

namespace MangaRipper.Forms
{
    public partial class FormMain : Form, IMainView
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private BindingList<DownloadChapterTask> _downloadQueue;
        private readonly ApplicationConfiguration _appConf = new ApplicationConfiguration();

        private string SaveDestination
        {
            get
            {
                return cbUseSeriesFolder.Checked ? SeriesDestination : txtSaveTo.Text;
            }
        }

        private string SeriesDestination
        {
            get;
            set;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        public void SetChaptersProgress(string progress)
        {
            txtPercent.Text = progress;
        }

        public void SetStatusText(string statusMessage)
        {
            txtMessage.Text = statusMessage;
        }

        public Func<string, Task> FindChaptersClicked { get; set; }

        public void SetChapters(IEnumerable<Chapter> chapters)
        {
            btnGetChapter.Enabled = true;
            dgvChapter.DataSource = chapters.ToList();
            PrefixLogic();
            PrepareSpecificDirectory();
        }

        private void btnGetChapter_Click(object sender, EventArgs e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("An Internet connection has not been detected.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error("Aborting chapter retrieval, no Internet connection.");
                return;
            }
            btnGetChapter.Enabled = false;
            var titleUrl = cbTitleUrl.Text;
            FindChaptersClicked(titleUrl);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var formats = GetOutputFormats().ToArray();
            if (formats.Length == 0)
            {
                MessageBox.Show("Please select at least one output format (Folder, Cbz...)");
                return;
            }
            var items = (from DataGridViewRow row in dgvChapter.Rows where row.Selected select row.DataBoundItem as Chapter).ToList();
            items = ApplicationConfiguration.DeepClone<IEnumerable<Chapter>>(items).ToList();
            items.Reverse();
            foreach (var item in items.Where(item => _downloadQueue.All(r => r.Chapter.Url != item.Url)))
                _downloadQueue.Add(new DownloadChapterTask(item, SaveDestination, formats));
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            var formats = GetOutputFormats().ToArray();
            if (formats.Length == 0)
            {
                MessageBox.Show("Please select at least one output format (Folder, Cbz...)");
                return;
            }

            var items = (from DataGridViewRow row in dgvChapter.Rows select (Chapter)row.DataBoundItem).ToList();
            items = ApplicationConfiguration.DeepClone<IEnumerable<Chapter>>(items).ToList();
            items.Reverse();
            foreach (var item in items.Where(item => _downloadQueue.All(r => r.Chapter.Url != item.Url)))
                _downloadQueue.Add(new DownloadChapterTask(item, SaveDestination, formats));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvQueueChapter.SelectedRows)
            {
                var chapter = (DownloadChapterTask)item.DataBoundItem;

                if (chapter.IsBusy == false)
                    _downloadQueue.Remove(chapter);
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            var removeItems = _downloadQueue.Where(r => r.IsBusy == false).ToList();

            foreach (var item in removeItems)
                _downloadQueue.Remove(item);
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                await StartDownload();
            }
            catch (OperationCanceledException ex)
            {
                txtMessage.Text = @"Download cancelled! Reason: " + ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMessage.Text = @"Download cancelled! Reason: " + ex.Message;
            }
            finally
            {
                btnDownload.Enabled = true;
            }
        }

        private async Task StartDownload()
        {
            while (_downloadQueue.Count > 0)
            {
                var chapter = _downloadQueue.First();
                var worker = FrameworkProvider.GetWorker();

                await worker.RunDownloadTaskAsync(chapter, new Progress<int>(c =>
                {
                    foreach (DataGridViewRow item in dgvQueueChapter.Rows)
                        if (chapter == item.DataBoundItem)
                        {
                            chapter.Percent = c;
                            dgvQueueChapter.Refresh();
                        }
                }));

                _downloadQueue.Remove(chapter);
            }
        }

        private IEnumerable<OutputFormat> GetOutputFormats()
        {
            var outputFormats = new List<OutputFormat>();

            if (cbSaveFolder.Checked)
                outputFormats.Add(OutputFormat.Folder);

            if (cbSaveCbz.Checked)
                outputFormats.Add(OutputFormat.CBZ);

            return outputFormats;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            FrameworkProvider.GetWorker().Cancel();
        }

        private void btnChangeSaveTo_Click(object sender, EventArgs e)
        {
            saveDestinationDirectoryBrowser.SelectedPath = SaveDestination;

            DialogResult dr = saveDestinationDirectoryBrowser.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                txtSaveTo.Text = saveDestinationDirectoryBrowser.SelectedPath;
            }

        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(SaveDestination))
            {
                Process.Start(SaveDestination);
            }
            else
            {
                MessageBox.Show($"Directory \"{SaveDestination}\" doesn't exist.");
            }
        }

        private void dgvSupportedSites_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                Process.Start(dgvSupportedSites.Rows[e.RowIndex].Cells[1].Value.ToString());
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Enables double-buffering to reduce flicker.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            var state = _appConf.LoadCommonSettings();
            Size = state.WindowSize;
            Location = state.Location;
            WindowState = state.WindowState;
            txtSaveTo.Text = state.SaveTo;
            cbTitleUrl.Text = state.Url;
            cbSaveCbz.Checked = state.CbzChecked;
            checkBoxForPrefix.Checked = state.PrefixChecked;

            dgvQueueChapter.AutoGenerateColumns = false;
            dgvChapter.AutoGenerateColumns = false;

            Text = $@"{Application.ProductName} {Application.ProductVersion}";

            try
            {
                foreach (var service in FrameworkProvider.GetMangaServices())
                {
                    var infor = service.GetInformation();
                    dgvSupportedSites.Rows.Add(infor.Name, infor.Link, infor.Language);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            if (string.IsNullOrWhiteSpace(SaveDestination))
                txtSaveTo.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _downloadQueue = _appConf.LoadDownloadChapterTasks();
            dgvQueueChapter.DataSource = _downloadQueue;
            LoadBookmark();
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            if (Application.ProductVersion == "1.0.0.0")
                return;

            var latestVersion = await UpdateNotification.GetLatestVersion();
            if (UpdateNotification.GetLatestBuildNumber(latestVersion) >
                UpdateNotification.GetLatestBuildNumber(Application.ProductVersion))
            {
                Logger.Info($"Local version: {Application.ProductVersion}. Remote version: {latestVersion}");

                if (MessageBox.Show(
                    $"There's a new version: ({latestVersion}) - Click OK to open download page.",
                    Application.ProductName,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
                {
                    Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/releases");
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var appConfig = _appConf.LoadCommonSettings();
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    appConfig.WindowSize = Size;
                    appConfig.Location = Location;
                    appConfig.WindowState = WindowState;
                    break;
                case FormWindowState.Maximized:
                    appConfig.WindowState = WindowState;
                    break;
                case FormWindowState.Minimized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            appConfig.Url = cbTitleUrl.Text;
            appConfig.SaveTo = txtSaveTo.Text;
            appConfig.CbzChecked = cbSaveCbz.Checked;
            appConfig.PrefixChecked = checkBoxForPrefix.Checked;
            _appConf.SaveCommonSettings(appConfig);
            _appConf.SaveDownloadChapterTasks(_downloadQueue);
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            // Method intentionally left empty.
        }

        private void LoadBookmark()
        {
            var bookmarks = _appConf.LoadBookMarks();
            cbTitleUrl.Items.Clear();
            var sc = bookmarks;
            if (sc == null) return;
            foreach (var item in sc)
                cbTitleUrl.Items.Add(item);
        }

        private void btnAddBookmark_Click(object sender, EventArgs e)
        {
            var sc = _appConf.LoadBookMarks().ToList();
            if (sc.Contains(cbTitleUrl.Text) == false)
            {
                sc.Add(cbTitleUrl.Text);
                _appConf.SaveBookmarks(sc);
                LoadBookmark();
            }
        }

        private void btnRemoveBookmark_Click(object sender, EventArgs e)
        {
            var sc = _appConf.LoadBookMarks().ToList();
            sc.Remove(cbTitleUrl.Text);
            _appConf.SaveBookmarks(sc);
            LoadBookmark();
        }

        private void txtSaveTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Reject the user's keystroke if it's an invalid character for paths.
            if ((Keys)e.KeyChar != Keys.Back && Path.GetInvalidPathChars().Contains(e.KeyChar))
            {
                // Display a tooltip telling the user their input has been rejected.
                FormToolTip.Show($"The character \"{e.KeyChar}\" is a invalid for use in paths.", txtSaveTo);

                e.Handled = true;
            }
            else
            {
                FormToolTip.SetToolTip(txtSaveTo, string.Empty);
            }
        }

        private void checkBoxForPrefix_CheckedChanged(object sender, EventArgs e)
        {
            PrefixLogic();
        }

        private void PrefixLogic()
        {
            var chapters = (from DataGridViewRow row in dgvChapter.Rows select row.DataBoundItem as Chapter).ToList();
            chapters.Reverse();
            chapters.ForEach(r => r.Prefix = checkBoxForPrefix.Checked ? chapters.IndexOf(r) + 1 : 0);
            chapters.Reverse();
            dgvChapter.DataSource = chapters;
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MangaRipper", "Data"));
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MangaRipper", "Logs"));
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/wiki");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.ShowDialog(this);
        }

        private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/wiki/Bug-Report");
        }

        /// <summary>
        /// Formulates a save destination based on the current series and selects it at the current series' save destination if it already exists.
        /// </summary>
        private void PrepareSpecificDirectory()
        {
            if (dgvChapter.RowCount == 0)
                return;

            var state = _appConf.LoadCommonSettings();

            string
                baseSeriesDestination = state.BaseSeriesDestination,
                series,
                seriesPath;

            if (!string.IsNullOrWhiteSpace(cbTitleUrl.Text))
            {
                Uri seriesUri;

                if (Uri.TryCreate(cbTitleUrl.Text, UriKind.Absolute, out seriesUri))
                    series = seriesUri.ToString();

                else
                    series = cbTitleUrl.SelectedItem.ToString();
            }
            else
            {
                series = cbTitleUrl.Text;
            }


            if (string.IsNullOrWhiteSpace(series))
            {
                // TODO Set series-specific directory path to the default value.
                return;
            }

            // If the base series destination hasn't been set, use MyDocuments as the base for now.
            if (string.IsNullOrEmpty(baseSeriesDestination))
                baseSeriesDestination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var item = (Chapter)dgvChapter.Rows[0].DataBoundItem;
            series = Core.Extensions.ExtensionHelper.RemoveFileNameInvalidChar(item.Name.Substring(0, item.Name.LastIndexOf(" ")).Trim());
            seriesPath = Path.Combine(baseSeriesDestination, series);

            SeriesDestination = seriesPath;

            FormToolTip.SetToolTip(cbUseSeriesFolder, $"Save chapters to {seriesPath}");

            /* 
             * Check if the series' directory exists and automatically check 'cbUseSeriesFolder' if it exists. Uncheck
             * it if it doesn't exist.
             * 
             * For the user's convenience, an option could allow saving to the series directory to be opt-out instead of opt-in.
             * Automatically putting each in its own directory could be troublesome for users who read a lot of one-shot manga.
            */
            cbUseSeriesFolder.Checked = Directory.Exists(seriesPath);

        }

        public void ShowMessageBox(string caption, string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(text, caption, buttons, icon);
        }
    }
}