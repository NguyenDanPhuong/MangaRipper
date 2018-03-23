using MangaRipper.Core.Models;
using System;

namespace MangaRipper.Core.Outputers
{
    public class OutputFactory : IOutputFactory
    {
        public IOutputer CreateOutput(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Folder:
                    return new FolderOutput();
                case OutputFormat.CBZ:
                    return new CbzOutput();
                case OutputFormat.Counter: // no IOutputer implementation
                    return null;
                default:
                    throw new NotImplementedException($"Unknow {nameof(OutputFormat)}: {Enum.GetName(typeof(OutputFormat), format)}");
            }
        }
    }
}
