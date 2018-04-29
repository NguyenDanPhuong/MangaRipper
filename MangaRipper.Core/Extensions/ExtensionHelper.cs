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
