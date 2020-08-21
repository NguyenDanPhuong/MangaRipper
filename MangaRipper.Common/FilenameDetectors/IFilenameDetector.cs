using System.Net.Http;
using System.Net.Http.Headers;

namespace MangaRipper.Core.FilenameDetectors
{
    public interface IFilenameDetector
    {
        string GetFilename(HttpResponseMessage response);
    }
}