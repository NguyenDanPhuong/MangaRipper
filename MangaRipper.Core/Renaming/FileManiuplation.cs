using System.IO;

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
