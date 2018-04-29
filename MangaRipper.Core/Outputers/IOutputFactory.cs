using MangaRipper.Core.Models;

namespace MangaRipper.Core.Outputers
{
    public interface IOutputFactory
    {
        IOutputer CreateOutput(OutputFormat format);
    }
}