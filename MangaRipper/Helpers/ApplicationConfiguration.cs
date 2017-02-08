using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MangaRipper.Core.Models;
using Newtonsoft.Json;
using NLog;

namespace MangaRipper.Helpers
{
    internal class ApplicationConfiguration
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
            return LoadObject<List<string>>(BookmarksFile);
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

        public void SaveDownloadChapterTasks(BindingList<DownloadChapterTask> tasks)
        {
            SaveObject(tasks, DownloadChapterTasksFile);
        }

        public BindingList<DownloadChapterTask> LoadDownloadChapterTasks()
        {
            return LoadObject<BindingList<DownloadChapterTask>>(DownloadChapterTasksFile);
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

            Logger.Info("> SaveObject(): " + fileName);
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, objectToStore);
            }
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
                Logger.Info("> LoadObject(): " + fileName);

                var serializer = new JsonSerializer();

                using (var sw = new StreamReader(fileName))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    result = serializer.Deserialize<T>(writer);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result == null ? Activator.CreateInstance<T>() : result;
        }

        public static T DeepClone<T>(T chapters)
        {
            var json = JsonConvert.SerializeObject(chapters);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}