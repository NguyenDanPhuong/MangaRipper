using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Renaming
{
    public class RenameByCounter : IRenamer
    {
        private readonly string folderPath;

        public RenameByCounter(string folderPath)
        {
            this.folderPath = folderPath;
        }
        public void Run()
        {
            var files = Directory.GetFiles(folderPath);
            for (int i = 0; i < files.Length; i++)
            {
                var file = new FileInfo(files[i]);
                var newFile = Path.Combine(file.DirectoryName, i.ToString() + file.Extension);
                File.Move(file.FullName, newFile);
            }
        }
    }
}
