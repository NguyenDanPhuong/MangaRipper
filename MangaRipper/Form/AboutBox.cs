using System;
using System.Windows.Forms;

namespace MangaRipper
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = string.Format("About {0}", AppInfo.AssemblyTitle);
            labelProductName.Text = AppInfo.AssemblyProduct;
            labelVersion.Text = string.Format("Version {0}", AppInfo.DeploymentVersion);
            labelCopyright.Text = AppInfo.AssemblyCopyright;
            labelCompanyName.Text = AppInfo.AssemblyCompany;
            textBoxDescription.Text = AppInfo.AssemblyDescription;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://mangaripper.codeplex.com/");
        }
    }
}
