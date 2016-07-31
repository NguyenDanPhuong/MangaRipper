using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using MangaRipper.Core;
using System.Windows.Forms;

namespace MangaRipper
{
    static class Common
    {
        /// <summary>
        /// Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="chapters"></param>
        /// <param name="fileName"></param>
        public static void SaveIChapterCollection(BindingList<Chapter> chapters, string fileName)
        {
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);
                using (var fs = new FileStream(file, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, chapters);
                }
            }
            catch
            {
                // Do nothings
            }
        }

        /// <summary>
        /// Load BindingList of IChapter from IsolateStorage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BindingList<Chapter> LoadIChapterCollection(string fileName)
        {
            BindingList<Chapter> result = null;
            try
            {
                string file = Path.Combine(Application.UserAppDataPath, fileName);
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length != 0)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        result = (BindingList<Chapter>)formatter.Deserialize(fs);
                    }
                }
            }
            catch
            {
                if (result == null)
                {
                    result = new BindingList<Chapter>();
                }
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