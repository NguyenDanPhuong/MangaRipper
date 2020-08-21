using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MangaRipper.Models;
using Newtonsoft.Json;
using NLog;

namespace MangaRipper.Helpers
{
    public class ApplicationConfiguration
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public readonly string AppDataPath;

        public readonly string DownloadChapterTasksFile;
        public readonly string CommonSettingsFile;
        public readonly string BookmarksFile;

        public ApplicationConfiguration()
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MangaRipper", "Data");
            Directory.CreateDirectory(AppDataPath);
            DownloadChapterTasksFile = Path.Combine(AppDataPath, "DownloadChapterTasks.json");
            CommonSettingsFile = Path.Combine(AppDataPath, "CommonSettings.json");
            BookmarksFile = Path.Combine(AppDataPath, "Bookmarks.json");
        }

        public IEnumerable<string> LoadBookMarks()
        {
            return LoadObject<List<string>>(BookmarksFile).OrderBy(s => s);
        }

        public void SaveBookmarks(IEnumerable<string> bookmarks)
        {
            SaveObject(bookmarks, BookmarksFile);
        }

        public CommonSettings LoadCommonSettings()
        {
            return LoadObject<CommonSettings>(CommonSettingsFile);
        }

        public void SaveCommonSettings(CommonSettings commonSettings)
        {
            SaveObject(commonSettings, CommonSettingsFile);
        }

        public void SaveDownloadChapterTasks(IEnumerable<DownloadRow> tasks)
        {
            foreach (var task in tasks)
            {
                task.IsBusy = false;
            }
            SaveObject(tasks, DownloadChapterTasksFile);
        }

        public IEnumerable<DownloadRow> LoadDownloadChapterTasks()
        {
            return LoadObject<List<DownloadRow>>(DownloadChapterTasksFile);
        }

        /// <summary>
        ///     Save object to a JSON file.
        /// </summary>
        /// <param name="objectToStore"></param>
        /// <param name="fileName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SaveObject<T>(T objectToStore, string fileName)
        {
            if (objectToStore == null)
            {
                throw new ArgumentNullException(nameof(objectToStore));
            }

            Logger.Info("> SaveObject(): " + Core.Extensions.ExtensionHelper.SanitizeUserName(fileName));
            var serializer = new JsonSerializer();
            using var sw = new StreamWriter(fileName);
            using JsonWriter writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(writer, objectToStore);
        }

        /// <summary>
        ///     Load object by deserialize a JSON file.
        /// </summary>
        /// <returns></returns>
        private T LoadObject<T>(string fileName) where T : new()
        {
            var result = default(T);
            try
            {
                Logger.Info("> LoadObject(): " + Core.Extensions.ExtensionHelper.SanitizeUserName(fileName));

                var serializer = new JsonSerializer();

                using var sw = new StreamReader(fileName);
                using JsonReader writer = new JsonTextReader(sw);
                result = serializer.Deserialize<T>(writer);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result == null ? Activator.CreateInstance<T>() : result;
        }
    }
}