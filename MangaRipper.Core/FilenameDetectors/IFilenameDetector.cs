using System.Net.Http.Headers;

namespace MangaRipper.Core.FilenameDetectors
{
    public interface IFilenameDetector
    {
        string GetFilename(string url, HttpContentHeaders headers);
    }
}