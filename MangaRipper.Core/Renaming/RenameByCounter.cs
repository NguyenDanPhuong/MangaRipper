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
        private readonly IFileManipulation fileManipulation;

        public RenameByCounter(IFileManipulation fileManipulation)
        {
            this.fileManipulation = fileManipulation;
        }

        public void Run(string folderPath)
        {
            var files = fileManipulation.GetFiles(folderPath);
            for (int i = 0; i < files.Length; i++)
            {
                var file = new FileInfo(files[i]);
                var newFile = Path.Combine(file.DirectoryName, (i + 1).ToString() + file.Extension);
                fileManipulation.Move(file.FullName, newFile);
            }
        }
    }
}
