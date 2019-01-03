using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Renaming
{
    public class FileManiuplation : IFileManipulation
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public void Move(string sourceFilename, string destFilename)
        {
            File.Move(sourceFilename, destFilename);
        }
    }
}
