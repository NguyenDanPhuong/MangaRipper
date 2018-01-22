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
                default:
                    throw new NotImplementedException($"Unknow {nameof(OutputFormat)}: {Enum.GetName(typeof(OutputFormat), format)}");
            }
        }
    }
}
