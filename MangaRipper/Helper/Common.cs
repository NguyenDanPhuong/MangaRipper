using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using MangaRipper.Core;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;

namespace MangaRipper
{
    static class Common
    {
        // TODO: Save CBZ, Folder checkbox settings.
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="fileName"></param>
        public static void SaveDownloadTasks(BindingList<DownloadChapterTask> tasks, string fileName)
        {
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);

                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(file))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, tasks);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Load BindingList of IChapter from IsolateStorage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BindingList<DownloadChapterTask> LoadDownloadTasks(string fileName)
        {
            BindingList<DownloadChapterTask> result = null;
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);

                JsonSerializer serializer = new JsonSerializer();

                using (StreamReader sw = new StreamReader(file))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    result = serializer.Deserialize<BindingList<DownloadChapterTask>>(writer);
                }
            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    result = new BindingList<DownloadChapterTask>();
                }
                _logger.Error(ex);
            }

            return result;
        }

        public static List<Chapter> CloneIChapterCollection(List<Chapter> chapters)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, chapters);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (List<Chapter>)formatter.Deserialize(objectStream);
            }
        }
    }
}