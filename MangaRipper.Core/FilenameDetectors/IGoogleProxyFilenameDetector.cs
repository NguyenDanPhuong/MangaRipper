namespace MangaRipper.Core.FilenameDetectors
{
    public interface IGoogleProxyFilenameDetector
    {
        string ParseFilename(string url);
    }
}