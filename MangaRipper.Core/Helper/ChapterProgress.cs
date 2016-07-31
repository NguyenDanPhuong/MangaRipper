namespace MangaRipper.Core
{
    public class ChapterProgress
    {
        public ChapterProgress(Chapter chapter, int percent)
        {
            Chapter = chapter;
            Percent = percent;
        }
        public Chapter Chapter
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
