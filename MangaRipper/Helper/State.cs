using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaRipper.Helper
{
    public class State
    {
        public string SaveTo { get; set; }
        public string Url { get; set; }
        public Size WindowSize { get; set; }
        public Point Location { get; set; }
        public FormWindowState WindowState { get; set; }
    }
}
