namespace MangaRipper.Core.Outputers
{
    public interface IOutputer
    {
        void CreateOutput(string sourceFolder, string destinationFolder);
    }
}