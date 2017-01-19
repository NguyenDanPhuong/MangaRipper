using MangaRipper.Core.Models;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRipper.Core.Helpers
{
    /// <summary>
    /// Looking for manga/chapter information in html, using regex.
    /// </summary>
    public class ParserHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Looking for chapter information in html code.
        /// </summary>
        /// <param name="regExp">The regex with 2 captured groups.</param>
        /// <param name="input">The html code.</param>
        /// <param name="nameGroup">The group name that capture chapter name.</param>
        /// <param name="valueGroup">The group name that capture chapter URL.</param>
        /// <returns></returns>
        public IEnumerable<Chapter> ParseGroup(string regExp, string input, string nameGroup, string valueGroup)
        {
            logger.Info("> ParseGroup: {0}", regExp);
            var list = new List<Chapter>();
            Regex reg = new Regex(regExp, RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(input);

            foreach (Match match in matches)
            {
                var value = match.Groups[valueGroup].Value.Trim();
                string name = match.Groups[nameGroup].Value.Trim();
                var chapter = new Chapter(name, value);
                list.Add(chapter);
            }

            return list.Distinct().ToList();
        }

        /// <summary>
        /// Using regex to capture a group with input html code.
        /// </summary>
        /// <param name="regExp">The regular expression</param>
        /// <param name="input">HTML string</param>
        /// <param name="groupName">Name of captured group. It's returned information.</param>
        /// <returns></returns>
        public IEnumerable<string> Parse(string regExp, string input, string groupName)
        {
            logger.Info("> Parse: {0}", regExp);
            var list = new List<string>();
            Regex reg = new Regex(regExp, RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(input);

            foreach (Match match in matches)
            {
                var value = match.Groups[groupName].Value.Trim();
                list.Add(value);
            }

            return list.Distinct().ToList();
        }
    }
}
