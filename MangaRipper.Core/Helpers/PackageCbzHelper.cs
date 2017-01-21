using System.IO.Compression;
using NLog;

namespace MangaRipper.Core.Helpers
{
    public class PackageCbzHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Create cbz file
        /// </summary>
        /// <param name="startPath">@"c:\example\start"</param>
        /// <param name="zipPath">@"c:\example\result.zip"</param>
        public static void Create(string startPath, string zipPath)
        {
            Logger.Info($@"Creating Zip from `{startPath}` to `{zipPath}`");
            ZipFile.CreateFromDirectory(startPath, zipPath);
        }
    }
}
