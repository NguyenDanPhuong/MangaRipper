using System.IO.Compression;
namespace MangaRipper.Core.Helpers
{
    public class PackageCbzHelper
    {
        /// <summary>
        /// Create cbz file
        /// </summary>
        /// <param name="startPath">@"c:\example\start"</param>
        /// <param name="zipPath">@"c:\example\result.zip"</param>
        public static void Create(string startPath, string zipPath)
        {
            ZipFile.CreateFromDirectory(startPath, zipPath);
        }
    }
}
