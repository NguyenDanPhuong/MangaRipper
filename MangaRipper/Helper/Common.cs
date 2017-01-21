using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MangaRipper.Core.Models;
using Newtonsoft.Json;
using NLog;

namespace MangaRipper
{
    internal class Common
    {
        // TODO: Save CBZ, Folder checkbox settings.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public readonly string AppDataPath;

        public readonly string DownloadChapterTasksFile;

        public Common()
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MangaRipper", "Data");
            Directory.CreateDirectory(AppDataPath);
            DownloadChapterTasksFile = Path.Combine(AppDataPath,
            "DownloadChapterTasks.json");
        }
        /// <summary>
        ///     Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="tasks"></param>
        public void SaveDownloadTasks(BindingList<DownloadChapterTask> tasks)
        {
            try
            {
                Logger.Info("> SaveDownloadTasks(): " + DownloadChapterTasksFile);
                var serializer = new JsonSerializer();
                using (var sw = new StreamWriter(DownloadChapterTasksFile))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, tasks);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        ///     Load BindingList of IChapter from IsolateStorage
        /// </summary>
        /// <returns></returns>
        public BindingList<DownloadChapterTask> LoadDownloadTasks()
        {
            BindingList<DownloadChapterTask> result = null;
            try
            {
                Logger.Info("> LoadDownloadTasks(): " + DownloadChapterTasksFile);

                var serializer = new JsonSerializer();

                using (var sw = new StreamReader(DownloadChapterTasksFile))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    result = serializer.Deserialize<BindingList<DownloadChapterTask>>(writer);
                }
            }
            catch (Exception ex)
            {
                if (result == null)
                    result = new BindingList<DownloadChapterTask>();
                Logger.Error(ex);
            }

            return result ?? new BindingList<DownloadChapterTask>();
        }

        public static IEnumerable<Chapter> CloneIChapterCollection(IEnumerable<Chapter> chapters)
        {
            var json = JsonConvert.SerializeObject(chapters);
            return JsonConvert.DeserializeObject<IEnumerable<Chapter>>(json);
        }
    }
}