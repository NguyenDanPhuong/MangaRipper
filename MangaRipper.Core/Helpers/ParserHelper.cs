using MangaRipper.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MangaRipper.Core.CustomException;

namespace MangaRipper.Core.Helpers
{
    /// <summary>
    /// Looking for manga/chapter information in html, using regex.
    /// </summary>
    public class ParserHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public delegate Chapter ChapterResolverHandler(string name, string value, Uri adress);

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
            Logger.Info("> ParseGroup: {0}", regExp);
            var list = new List<Chapter>();
            var reg = new Regex(regExp, RegexOptions.IgnoreCase);
            var matches = reg.Matches(input);

            if (matches.Count == 0)
            {
                Logger.Error("Cannot parse below content.");
                Logger.Error(input);
                throw new MangaRipperException("Parse content failed! Please check if you can access this content on your browser.");
            }

            foreach (Match match in matches)
            {
                var value = match.Groups[valueGroup].Value.Trim();
                var name = match.Groups[nameGroup].Value.Trim();
                var chapter = new Chapter(name, value);
                list.Add(chapter);
            }
            var result = list.Distinct().ToList();
            Logger.Info($@"Parse success. There are {result.Count()} item(s).");
            return result;
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
            Logger.Info("> Parse: {0}", regExp);
            var reg = new Regex(regExp, RegexOptions.IgnoreCase);
            var matches = reg.Matches(input);

            if (matches.Count == 0)
            {
                Logger.Error("Cannot parse below content.");
                Logger.Error(input);
                throw new MangaRipperException("Parse content failed! Please check if you can access this content on your browser.");
            }

            var list = (from Match match in matches select match.Groups[groupName].Value.Trim()).ToList();
            var result = list.Distinct().ToList();
            Logger.Info($@"Parse success. There are {result.Count()} item(s).");
            return result;
        }
      
    }
}
