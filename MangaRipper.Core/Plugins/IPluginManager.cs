namespace MangaRipper.Core.Plugins
{
    public interface IPluginManager
    {
        IMangaPlugin GetService(string link);
    }
}