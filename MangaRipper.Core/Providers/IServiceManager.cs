using MangaRipper.Core.Interfaces;

namespace MangaRipper.Core.Providers
{
    public interface IServiceManager
    {
        IMangaService GetService(string link);
    }
}