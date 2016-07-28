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
        BindingList<IChapter> DownloadQueue;

        protected const string FILENAME_ICHAPTER_COLLECTION = "IChapterCollection.bin";

        private CancellationTokenSource _cts;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnGetChapter_Click(object sender, EventArgs e)
        {
            try
            {
                var titleUrl = cbTitleUrl.Text;
                ITitle title = TitleFactory.CreateTitle(titleUrl);
                title.Proxy = Option.GetProxy();
                btnGetChapter.Enabled = false;
                var task = title.PopulateChapterAsync(new Progress<int>(progress => txtPercent.Text = progress + "%"));
                task.ContinueWith(t =>
                {
                    btnGetChapter.Enabled = true;
                    dgvChapter.DataSource = title.Chapters;

                    if (t.Exception != null && t.Exception.InnerException != null)
                    {
                        txtMessage.Text = t.Exception.InnerException.Message;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var items = new List<IChapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                if (row.Selected == true)
                {
                    items.Add((IChapter)row.DataBoundItem);
                }
            }

            items.Reverse();
            foreach (IChapter item in items)
            {
                if (DownloadQueue.IndexOf(item) < 0)
                {
                    DownloadQueue.Add(item);
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            var items = new List<IChapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                items.Add((IChapter)row.DataBoundItem);
            }
            items.Reverse();
            foreach (IChapter item in items)
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
                IChapter chapter = (IChapter)item.DataBoundItem;
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            DownloadChapter();
        }


        private void DownloadChapter(int millisecond)
        {
            timer1.Interval = millisecond;
            timer1.Enabled = true;
        }

        private void DownloadChapter()
        {
            if (DownloadQueue.Count > 0 && _cts.IsCancellationRequested == false)
            {
                int current = DownloadQueue.Where(c => c.IsBusy == true).Count();
                int max = Convert.ToInt32(nudThread.Value);
                int remain = max - current;
                var chapters = DownloadQueue.Where(c => c.IsBusy == false).Take(remain);

                foreach (var chapter in chapters)
                {
                    chapter.Proxy = Option.GetProxy();
                    btnDownload.Enabled = false;
                    var task = chapter.DownloadImageAsync(txtSaveTo.Text, _cts.Token, new Progress<ChapterProgress>(c =>
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

                    task.ContinueWith(t =>
                    {
                        switch (t.Status)
                        {
                            case TaskStatus.Canceled:
                                btnDownload.Enabled = true;
                                break;
                            case TaskStatus.Faulted:
                                txtMessage.Text = t.Exception.InnerException.Message;
                                DownloadChapter(1000);
                                break;
                            case TaskStatus.RanToCompletion:
                                DownloadQueue.Remove(chapter);
                                DownloadChapter();
                                break;
                            default:
                                break;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                btnDownload.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
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

            foreach (string[] item in TitleFactory.GetSupportedSites())
            {
                dgvSupportedSites.Rows.Add(item);
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
            Process.Start("http://mangaripper.codeplex.com/documentation");
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

        private void btnOptions_Click(object sender, EventArgs e)
        {
            FormOption form = new FormOption();
            form.ShowDialog(this);
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
            var chapters = new List<IChapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                IChapter chapter = row.DataBoundItem as IChapter;
                chapters.Add(chapter);
            }
            chapters = Common.CloneIChapterCollection(chapters);

            chapters.Reverse();
            chapters.ForEach(r => r.Name = string.Format("[{0:000}] - {1}", chapters.IndexOf(r) + 1, r.Name));
            chapters.Reverse();

            dgvChapter.DataSource = chapters;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            DownloadChapter();
        }
    }
}
