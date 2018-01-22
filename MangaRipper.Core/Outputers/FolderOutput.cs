using MangaRipper.Core.Extensions;
using System.IO;

namespace MangaRipper.Core.Outputers
{
    class FolderOutput : IOutputer
    {
        public void CreateOutput(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            ExtensionHelper.SuperMove(sourceFolder, destinationFolder);
        }
    }
}
