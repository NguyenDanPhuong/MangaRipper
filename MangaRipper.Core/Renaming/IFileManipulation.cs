using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Renaming
{
    public interface IFileManipulation
    {
        void Move(string sourceFilename, string destFilename);
        string[] GetFiles(string path);
    }
}
