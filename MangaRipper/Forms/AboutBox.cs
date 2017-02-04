using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MangaRipper.Forms
{
    internal sealed partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = $@"About {Application.ProductName}";
            labelProductName.Text = Application.ProductName;
            labelVersion.Text = $@"Version {Application.ProductVersion}";
            labelCopyright.Text = Application.CompanyName;
            labelCompanyName.Text = @"Copyright © 2017";
            textBoxDescription.Text = @"This software helps you download manga (Japanese Comic) from several websites for your offline viewing.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NguyenDanPhuong/MangaRipper");
        }
    }
}