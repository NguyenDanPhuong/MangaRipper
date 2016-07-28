namespace MangaRipper.Core
{
    public class ChapterProgress
    {
        public ChapterProgress(IChapter chapter, int percent)
        {
            Chapter = chapter;
            Percent = percent;
        }
        public IChapter Chapter
        {
            get;
            private set;
        }

        public int Percent
        {
            get;
            private set;
        }
    }
}
