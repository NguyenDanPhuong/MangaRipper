namespace MangaRipper.Core
{
    public class MyLogger
    {
        private MyLogger()
        {

        }
        public static MyLogger CreateLogger()
        {
            return new MyLogger();
        }
    }
}
