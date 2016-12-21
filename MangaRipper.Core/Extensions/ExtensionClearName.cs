namespace MangaRipper.Core.Extensions
{
    public static class ExtensionClearName
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
    }
}
