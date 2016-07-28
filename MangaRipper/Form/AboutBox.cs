using System;
using System.Windows.Forms;

namespace MangaRipper
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = string.Format("About {0}", "MangaRipper");
            labelProductName.Text = "2012";
            labelVersion.Text = string.Format("Version {0}", 2012);
            labelCopyright.Text = "NguyenDanPhuong";
            labelCompanyName.Text = "NguyenDanPhuong";
            textBoxDescription.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://mangaripper.codeplex.com/");
        }
    }
}
