using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using MangaRipper.Core;

namespace MangaRipper
{
    static class Common
    {
        /// <summary>
        /// Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="chapters"></param>
        /// <param name="fileName"></param>
        public static void SaveIChapterCollection(BindingList<IChapter> chapters, string fileName)
        {
            try
            {
                using (IsolatedStorageFile scope = IsolatedStorageFile.GetUserStoreForApplication())
                using (var fs = new IsolatedStorageFileStream(fileName, FileMode.Create, scope))
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
        public static BindingList<IChapter> LoadIChapterCollection(string fileName)
        {
            BindingList<IChapter> result = null;
            try
            {
                using (IsolatedStorageFile scope = IsolatedStorageFile.GetUserStoreForApplication())
                using (var fs = new IsolatedStorageFileStream(fileName, FileMode.Open, scope))
                {
                    if (fs.Length != 0)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        result = (BindingList<IChapter>)formatter.Deserialize(fs);
                    }
                }
            }
            catch
            {
                if (result == null)
                {
                    result = new BindingList<IChapter>();
                }
            }

            return result;
        }

        public static List<IChapter> CloneIChapterCollection(List<IChapter> chapters)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, chapters);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (List<IChapter>)formatter.Deserialize(objectStream);
            }
        }
    }
}