using System.Drawing;
using System.Windows.Forms;

namespace MangaRipper.Helpers
{
    public class CommonSettings
    {
        public string SaveTo { get; set; }
        public string Url { get; set; }
        public Size WindowSize { get; set; }
        public Point Location { get; set; }
        public FormWindowState WindowState { get; set; }
        public bool CbzChecked { get; set; }
    }
}
