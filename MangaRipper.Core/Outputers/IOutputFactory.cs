using MangaRipper.Core.Models;

namespace MangaRipper.Core.Outputers
{
    public interface IOutputFactory
    {
        IOutputer Create(OutputFormat format);
    }
}