using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Renaming
{
    public interface IFileManipulation
    {
        void Move(string sourcePath, string destinationPath);
        void Rename(string original, string newName);
        string[] GetFiles(string path);
    }
}
