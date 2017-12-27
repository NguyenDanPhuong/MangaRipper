using MangaRipper.Core.Interfaces;
using System.IO.Compression;

namespace MangaRipper.Core.Helpers
{
    public class PackageCbzHelper
    {
        private readonly ILogger logger;

        public PackageCbzHelper(ILogger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Create cbz file
        /// </summary>
        /// <param name="startPath">@"c:\example\start"</param>
        /// <param name="zipPath">@"c:\example\result.zip"</param>
        public void Create(string startPath, string zipPath)
        {
            logger.Info($@"Creating Zip from '{startPath}' to '{zipPath}'");
            ZipFile.CreateFromDirectory(startPath, zipPath);
        }
    }
}
