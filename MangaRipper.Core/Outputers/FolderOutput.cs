using System.IO;

namespace MangaRipper.Core.Outputers
{
    class FolderOutput : IOutputer
    {
        public void CreateOutput(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            SuperMove(sourceFolder, destinationFolder);
        }

        /// <summary>
        /// Directory.Move in C# cannot move across disk volume.
        /// So we have this SUPER MOVE!!! :)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        private void SuperMove(string sourcePath, string destinationPath)
        {
            CopyFolderAndAllSubItems(new DirectoryInfo(sourcePath), new DirectoryInfo(destinationPath));
        }

        private void CopyFolderAndAllSubItems(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFolderAndAllSubItems(dir, destination.CreateSubdirectory(dir.Name));
            }
            foreach (FileInfo file in source.GetFiles())
            {
                string destFileName = Path.Combine(destination.FullName, file.Name);
                if (!File.Exists(destFileName))
                {
                    file.CopyTo(destFileName);
                }
            }
        }
    }
}
