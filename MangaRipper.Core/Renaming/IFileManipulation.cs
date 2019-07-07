namespace MangaRipper.Core.Renaming
{
    public interface IFileManipulation
    {
        void Move(string sourceFilename, string destFilename);
        string[] GetFiles(string path);
    }
}
