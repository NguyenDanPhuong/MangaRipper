using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Deployment.Application;

namespace MangaRipper
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AppInfo.AssemblyTitle);
            this.labelProductName.Text = AppInfo.AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AppInfo.DeploymentVersion);
            this.labelCopyright.Text = AppInfo.AssemblyCopyright;
            this.labelCompanyName.Text = AppInfo.AssemblyCompany;
            this.textBoxDescription.Text = AppInfo.AssemblyDescription;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://mangaripper.codeplex.com/");
        }
    }
}
