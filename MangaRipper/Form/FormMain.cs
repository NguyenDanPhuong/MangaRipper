using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Specialized;
using MangaRipper.Core;
using System.Threading.Tasks;
using NLog;

namespace MangaRipper
{
    public partial class FormMain : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        BindingList<DownloadChapterTask> _downloadQueue;
        protected const string FilenameIchapterCollection = "IChapterCollection.bin";

        public FormMain()
        {
            InitializeComponent();
        }

        private async void btnGetChapter_Click(object sender, EventArgs e)
        {
            try
            {
                btnGetChapter.Enabled = false;
                var titleUrl = cbTitleUrl.Text;

                var worker = Framework.GetWorker();
                var progressInt = new Progress<int>(progress => txtPercent.Text = progress + "%");
                var chapters = await worker.FindChapters(titleUrl, progressInt);
                dgvChapter.DataSource = chapters;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                txtMessage.Text = @"Download cancelled! Reason: " + ex.Message;
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                btnGetChapter.Enabled = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var items = new List<Chapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                if (row.Selected)
                {
                    items.Add((Chapter)row.DataBoundItem);
                }
            }

            items.Reverse();
            foreach (Chapter item in items)
            {
                if (_downloadQueue.All(r => r.Chapter.Url != item.Url))
                {
                    _downloadQueue.Add(new DownloadChapterTask(item, txtSaveTo.Text, GetOutputFormats()));
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            var items = new List<Chapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                items.Add((Chapter)row.DataBoundItem);
            }
            items.Reverse();
            foreach (Chapter item in items)
            {
                if (_downloadQueue.All(r => r.Chapter.Url != item.Url))
                {
                    _downloadQueue.Add(new DownloadChapterTask(item, txtSaveTo.Text, GetOutputFormats()));
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvQueueChapter.SelectedRows)
            {
                DownloadChapterTask chapter = (DownloadChapterTask)item.DataBoundItem;
                if (chapter.IsBusy == false)
                {
                    _downloadQueue.Remove(chapter);
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            var removeItems = _downloadQueue.Where(r => r.IsBusy == false).ToList();

            foreach (var item in removeItems)
            {
                _downloadQueue.Remove(item);
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                await StartDownload();
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
                var worker = Framework.GetWorker();

                await worker.Run(chapter, new Progress<int>(c =>
                    {
                        foreach (DataGridViewRow item in dgvQueueChapter.Rows)
                        {
                            if (chapter == item.DataBoundItem)
                            {
                                chapter.Percent = c;
                                dgvQueueChapter.Refresh();
                            }
                        }
                    }));

                _downloadQueue.Remove(chapter);
            }
        }

        private IEnumerable<OutputFormat> GetOutputFormats()
        {
            var outputFormats = new List<OutputFormat>();
            if (cbSaveFolder.Checked)
            {
                outputFormats.Add(OutputFormat.Folder);
            }
            if (cbSaveCbz.Checked)
            {
                outputFormats.Add(OutputFormat.CBZ);
            }
            return outputFormats;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Framework.GetWorker().Cancel();
        }

        private void btnChangeSaveTo_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtSaveTo.Text;
            DialogResult dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                txtSaveTo.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            Process.Start(txtSaveTo.Text);
        }

        private void dgvSupportedSites_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                Process.Start(dgvSupportedSites.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Size = Properties.Settings.Default.Size;
            Location = Properties.Settings.Default.Location;
            WindowState = Properties.Settings.Default.WindowState;

            dgvQueueChapter.AutoGenerateColumns = false;
            dgvChapter.AutoGenerateColumns = false;

            Text = $"{Application.ProductName} {Application.ProductVersion}";

            foreach (var service in Framework.GetServices())
            {
                var infor = service.GetInformation();
                dgvSupportedSites.Rows.Add(infor.Name, infor.Link, infor.Language);
            }

            if (string.IsNullOrEmpty(txtSaveTo.Text))
            {
                txtSaveTo.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            _downloadQueue = Common.LoadDownloadTasks(FilenameIchapterCollection);
            dgvQueueChapter.DataSource = _downloadQueue;

            LoadBookmark();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.ShowDialog(this);
        }

        private void btnHowToUse_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper/wiki");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Size = Size;
                Properties.Settings.Default.Location = Location;
                Properties.Settings.Default.WindowState = WindowState;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.WindowState = WindowState;
            }

            Properties.Settings.Default.Save();
            Common.SaveDownloadTasks(_downloadQueue, FilenameIchapterCollection);
        }

        private void LoadBookmark()
        {
            cbTitleUrl.Items.Clear();
            var sc = Properties.Settings.Default.Bookmark;
            if (sc != null)
            {
                foreach (string item in sc)
                {
                    cbTitleUrl.Items.Add(item);
                }
            }
        }

        private void btnAddBookmark_Click(object sender, EventArgs e)
        {
            var sc = Properties.Settings.Default.Bookmark ?? new StringCollection();
            if (sc.Contains(cbTitleUrl.Text) == false)
            {
                sc.Add(cbTitleUrl.Text);
                Properties.Settings.Default.Bookmark = sc;
                LoadBookmark();
            }
        }

        private void btnRemoveBookmark_Click(object sender, EventArgs e)
        {
            var sc = Properties.Settings.Default.Bookmark;
            if (sc != null)
            {
                sc.Remove(cbTitleUrl.Text);
                Properties.Settings.Default.Bookmark = sc;
                LoadBookmark();
            }
        }

        private void btnAddPrefixCounter_Click(object sender, EventArgs e)
        {
            var chapters = new List<Chapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                var chapter = row.DataBoundItem as Chapter;
                chapters.Add(chapter);
            }
            chapters = Common.CloneIChapterCollection(chapters).ToList();

            chapters.Reverse();
            chapters.ForEach(r => r.AddPrefix(chapters.IndexOf(r) + 1));
            chapters.Reverse();

            dgvChapter.DataSource = chapters;
        }

    }
}
