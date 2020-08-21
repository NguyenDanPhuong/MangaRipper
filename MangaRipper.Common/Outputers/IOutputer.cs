namespace MangaRipper.Core.Outputers
{
    public interface IOutputer
    {
        void Save(string sourceFolder, string destinationFolder);
    }
}