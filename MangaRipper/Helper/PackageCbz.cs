using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
namespace MangaRipper.Helper
{
    public class PackageCbz
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
