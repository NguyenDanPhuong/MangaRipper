using System;
using System.Windows.Forms;

namespace MangaRipper
{
    sealed partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = $"About {Application.ProductName}";
            labelProductName.Text = Application.ProductName;
            labelVersion.Text = $"Version {Application.ProductVersion}";
            labelCopyright.Text = Application.CompanyName;
            labelCompanyName.Text = @"Copyright © 2011";
            textBoxDescription.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/NguyenDanPhuong/MangaRipper");
        }
    }
}
