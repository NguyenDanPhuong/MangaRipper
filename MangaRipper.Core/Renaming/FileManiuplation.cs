using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Renaming
{
    class FileManiuplation : IFileManipulation
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public void Move(string sourcePath, string destinationPath)
        {
        }

        public void Rename(string original, string newName)
        {
        }
    }
}
