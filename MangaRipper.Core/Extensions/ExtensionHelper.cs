using System.IO;

namespace MangaRipper.Core.Extensions
{
    public static class ExtensionHelper
    {
        /// <summary>
        /// Remove characters that cannot using to name folder, file.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveFileNameInvalidChar(this string input)
        {
            return input.Replace("\\", "").Replace("/", "").Replace(":", "")
                        .Replace("*", "").Replace("?", "").Replace("\"", "")
                        .Replace("<", "").Replace(">", "").Replace("|", "");
        }

        /// <summary>
        /// Directory.Move in C# cannot move across disk volume.
        /// So we have this SUPER MOVE!!! :)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public static void SuperMove(string sourcePath, string destinationPath)
        {
            CopyFolderAndAllSubItems(new DirectoryInfo(sourcePath), new DirectoryInfo(destinationPath));
        }

        public static void CopyFolderAndAllSubItems(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFolderAndAllSubItems(dir, destination.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
        }

        /// <summary>
        /// Replaces the user's name with a generic placeholder to protect their privacy.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SanitizeUserName(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Replace(System.Environment.UserName, "<user>");
            }
            else
            {
                throw new System.ArgumentNullException("Value cannot be null.");
            }

        }
    }
}
