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

namespace MangaRipper
{
    static class Common
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="fileName"></param>
        public static void SaveIChapterCollection(BindingList<DownloadChapterTask> tasks, string fileName)
        {
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);
                using (var fs = new FileStream(file, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, tasks);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Load BindingList of IChapter from IsolateStorage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BindingList<DownloadChapterTask> LoadIChapterCollection(string fileName)
        {
            BindingList<DownloadChapterTask> result = null;
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length != 0)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        result = (BindingList<DownloadChapterTask>)formatter.Deserialize(fs);
                    }
                }
            }
            catch(Exception ex)
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