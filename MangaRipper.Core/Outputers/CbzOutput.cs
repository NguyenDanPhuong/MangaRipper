using System.IO.Compression;

namespace MangaRipper.Core.Outputers
{
    class CbzOutput : IOutputer
    {
        public void Save(string sourceFolder, string destinationFolder)
        {
            ZipFile.CreateFromDirectory(sourceFolder, destinationFolder + ".cbz");
        }
    }
}
