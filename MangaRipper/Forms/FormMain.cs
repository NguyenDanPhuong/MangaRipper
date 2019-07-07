using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core.Models;
using MangaRipper.Helpers;
using MangaRipper.Presenters;
using NLog;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Controllers;
using MangaRipper.Core.Extensions;
using MangaRipper.Models;

namespace MangaRipper.Forms
{
    public partial class FormMain : Form, IMainView
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private BindingList<DownloadRow> _downloadQueue;
        private readonly ApplicationConfiguration _appConf = new ApplicationConfiguration();

        private MainViewPresenter Presenter;
        private IEnumerable<IMangaService> MangaServices;
        private WorkerController worker;

        public FormMain(IEnumerable<IMangaService> mangaServices, WorkerController wc)
        {
            InitializeComponent();
            MangaServices = mangaServices;
            worker = wc;
            Presenter = new MainViewPresenter(this, wc);
        }

        public void SetChaptersProgress(string progress)
        {
            txtPercent.Text = progress;
        }

        public void SetStatusText(string statusMessage)
        {
            txtMessage.Text = statusMessage;
        }

        public void SetChapters(IEnumerable<Chapter> chapters)
        {
            btnGetChapter.Enabled = true;
            dgvChapter.DataSource = chapters.Select(c => new ChapterRow(c)).ToList();
            PrefixLogic();
        }

        private async void BtnGetChapter_ClickAsync(object sender, EventArgs e)
        {
            btnGetChapter.Enabled = false;
            var titleUrl = cbTitleUrl.Text;
            await Presenter.OnFindChapters(titleUrl);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var formats = GetOutputFormats().ToArray();
            if (formats.Length == 0)
            {
                MessageBox.Show("Please select at least one output format (Folder, Cbz...)");
                return;
            }
            var items = (from DataGridViewRow row in dgvChapter.Rows where row.Selected select row.DataBoundItem as ChapterRow).ToList();
            items.Reverse();
            foreach (var chapter in items.Where(i => _downloadQueue.All(r => r.Url != i.Url)))
            {
                var savePath = GetSavePath(chapter);
                var task = new DownloadRow
                {
                    Name = chapter.Name,
                    Url = chapter.Url,
                    SaveToFolder = savePath,
                    Formats = formats
                };
                _downloadQueue.Add(task);
            }
        }

        private string GetSavePath(ChapterRow chapter)
        {
            return Path.Combine(txtSaveTo.Text, chapter.DisplayName.RemoveFileNameInvalidChar());
        }

        private void BtnAddAll_Click(object sender, EventArgs e)
        {
            var formats = GetOutputFormats().ToArray();
            if (formats.Length == 0)
            {
                MessageBox.Show("Please select at least one output format (Folder, Cbz...)");
                return;
            }

            var items = (from DataGridViewRow row in dgvChapter.Rows select (ChapterRow)row.DataBoundItem).ToList();
            items.Reverse();
            foreach (var chapter in items.Where(item => _downloadQueue.All(r => r.Url != item.Url)))
            {
                var savePath = GetSavePath(chapter);
                var task = new DownloadRow
                {
                    Name = chapter.Name,
                    Url = chapter.Url,
                    SaveToFolder = savePath,
                    Formats = formats
                };
                _downloadQueue.Add(task);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvQueueChapter.SelectedRows)
            {
                var chapter = (DownloadRow)item.DataBoundItem;

                if (chapter.IsBusy == false)
                    _downloadQueue.Remove(chapter);
            }
        }

        private void BtnRemoveAll_Click(object sender, EventArgs e)
        {
            var removeItems = _downloadQueue.Where(r => r.IsBusy == false).ToList();

            foreach (var item in removeItems)
                _downloadQueue.Remove(item);
        }

        private async void BtnDownload_Click(object sender, EventArgs e)
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

                var task = new DownloadChapterTask(chapter.Name, chapter.Url, chapter.SaveToFolder, chapter.Formats);

                chapter.IsBusy = true;
                await worker.RunDownloadTaskAsync(task, new Progress<int>(c =>
                {
                    foreach (DataGridViewRow item in dgvQueueChapter.Rows)
                        if (chapter == item.DataBoundItem)
                        {
                            chapter.Percent = c;
                            dgvQueueChapter.Refresh();
                        }
                }));
                chapter.IsBusy = false;
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

        private void BtnStop_Click(object sender, EventArgs e)
        {
            worker.Cancel();
        }

        private void BtnChangeSaveTo_Click(object sender, EventArgs e)
        {
            saveDestinationDirectoryBrowser.SelectedPath = txtSaveTo.Text;

            DialogResult dr = saveDestinationDirectoryBrowser.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                txtSaveTo.Text = saveDestinationDirectoryBrowser.SelectedPath;
            }

        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtSaveTo.Text))
            {
                Process.Start(txtSaveTo.Text);
            }
            else
            {
                MessageBox.Show($"Directory \"{txtSaveTo.Text}\" doesn't exist.");
            }
        }

        private void DgvSupportedSites_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

            dgvQueueChapter.AutoGenerateColumns = false;
            dgvChapter.AutoGenerateColumns = false;

            Text = $@"{Application.ProductName} {Application.ProductVersion}";

            try
            {
                foreach (var service in MangaServices)
                {
                    var infor = service.GetInformation();
                    dgvSupportedSites.Rows.Add(infor.Name, infor.Link, infor.Language);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            if (string.IsNullOrWhiteSpace(txtSaveTo.Text))
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

        private void BtnAddBookmark_Click(object sender, EventArgs e)
        {
            var sc = _appConf.LoadBookMarks().ToList();
            if (sc.Contains(cbTitleUrl.Text) == false)
            {
                sc.Add(cbTitleUrl.Text);
                _appConf.SaveBookmarks(sc);
                LoadBookmark();
            }
        }

        private void BtnRemoveBookmark_Click(object sender, EventArgs e)
        {
            var sc = _appConf.LoadBookMarks().ToList();
            sc.Remove(cbTitleUrl.Text);
            _appConf.SaveBookmarks(sc);
            LoadBookmark();
        }

        private void TxtSaveTo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void CheckBoxForPrefix_CheckedChanged(object sender, EventArgs e)
        {
            PrefixLogic();
        }

        private void PrefixLogic()
        {
            var chapters = (from DataGridViewRow row in dgvChapter.Rows select row.DataBoundItem as ChapterRow).ToList();
            chapters.Reverse();
            chapters.ForEach(r => r.Prefix = checkBoxForPrefix.Checked ? chapters.IndexOf(r) + 1 : 0);
            chapters.Reverse();
            dgvChapter.DataSource = chapters;
        }

        private void DataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MangaRipper", "Data"));
        }

        private void LogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MangaRipper", "Logs"));
        }

        private void WikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/wiki");
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.ShowDialog(this);
        }

        private void BugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/wiki/Bug-Report");
        }

        public void ShowMessageBox(string caption, string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(text, caption, buttons, icon);
        }

        public void EnableTheButtonsAfterError()
        {
            btnGetChapter.Enabled = true;
            btnDownload.Enabled = true;
        }

        private void ContributorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/graphs/contributors");
        }
    }
}