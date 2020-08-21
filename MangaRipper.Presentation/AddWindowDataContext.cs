using MangaRipper.Core;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;
using MangaRipper.Helpers;
using MangaRipper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MangaRipper.Presentation
{
    public class AddWindowDataContext : BaseViewModel
    {
        public string SaveToFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public bool IsFolderSelected { get; set; } = true;
        public bool IsCbzSelected { get; set; }
    }
}
