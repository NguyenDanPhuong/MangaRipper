namespace MangaRipper.Core.Plugins
{
    public interface IPluginManager
    {
        IPlugin GetPlugin(string url);
    }
}