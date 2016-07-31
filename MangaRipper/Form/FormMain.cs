using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Specialized;
using MangaRipper.Core;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper
{
    public partial class FormMain : Form
    {
        BindingList<Chapter> DownloadQueue;

        protected const string FILENAME_ICHAPTER_COLLECTION = "IChapterCollection.bin";

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
                var progress_int = new Progress<int>(progress => txtPercent.Text = progress + "%");
                var chapters = await worker.FindChapters(titleUrl, progress_int);
                dgvChapter.DataSource = chapters;
            }
            catch (Exception ex)
            {
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
                if (row.Selected == true)
                {
                    items.Add((Chapter)row.DataBoundItem);
                }
            }

            items.Reverse();
            foreach (Chapter item in items)
            {
                if (DownloadQueue.IndexOf(item) < 0)
                {
                    DownloadQueue.Add(item);
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
                if (DownloadQueue.IndexOf(item) < 0)
                {
                    DownloadQueue.Add(item);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvQueueChapter.SelectedRows)
            {
                Chapter chapter = (Chapter)item.DataBoundItem;
                if (chapter.IsBusy == false)
                {
                    DownloadQueue.Remove(chapter);
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            var removeItems = DownloadQueue.Where(r => r.IsBusy == false).ToList();

            foreach (var item in removeItems)
            {
                DownloadQueue.Remove(item);
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

            }
            finally
            {
                btnDownload.Enabled = true;
            }
        }

        private async Task StartDownload()
        {
            while (DownloadQueue.Count > 0)
            {
                var chapter = DownloadQueue.First();
                var worker = Framework.GetWorker();
                await worker.DownloadChapter(chapter, txtSaveTo.Text, new Progress<ChapterProgress>(c =>
                    {
                        foreach (DataGridViewRow item in dgvQueueChapter.Rows)
                        {
                            if (c.Chapter == item.DataBoundItem)
                            {
                                item.Cells[ColChapterStatus.Name].Value = c.Percent + "%";
                                break;
                            }
                        }
                    }));

                DownloadQueue.Remove(chapter);
            }
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

            Text = string.Format("{0} {1}", Application.ProductName, Application.ProductVersion);

            foreach (var service in Framework.GetServices())
            {
                var infor = service.GetInformation();
                dgvSupportedSites.Rows.Add(infor.Name, infor.Link, infor.Language);
            }

            if (string.IsNullOrEmpty(txtSaveTo.Text))
            {
                txtSaveTo.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            DownloadQueue = Common.LoadIChapterCollection(FILENAME_ICHAPTER_COLLECTION);
            dgvQueueChapter.DataSource = DownloadQueue;

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
            Common.SaveIChapterCollection(DownloadQueue, FILENAME_ICHAPTER_COLLECTION);
        }

        private void LoadBookmark()
        {
            cbTitleUrl.Items.Clear();
            StringCollection sc = Properties.Settings.Default.Bookmark;
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
            StringCollection sc = Properties.Settings.Default.Bookmark;
            if (sc == null)
            {
                sc = new StringCollection();
            }
            if (sc.Contains(cbTitleUrl.Text) == false)
            {
                sc.Add(cbTitleUrl.Text);
                Properties.Settings.Default.Bookmark = sc;
                LoadBookmark();
            }
        }

        private void btnRemoveBookmark_Click(object sender, EventArgs e)
        {
            StringCollection sc = Properties.Settings.Default.Bookmark;
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
                Chapter chapter = row.DataBoundItem as Chapter;
                chapters.Add(chapter);
            }
            chapters = Common.CloneIChapterCollection(chapters);

            chapters.Reverse();
            chapters.ForEach(r => r.AddPrefix(chapters.IndexOf(r) + 1));
            chapters.Reverse();

            dgvChapter.DataSource = chapters;
        }

    }
}
