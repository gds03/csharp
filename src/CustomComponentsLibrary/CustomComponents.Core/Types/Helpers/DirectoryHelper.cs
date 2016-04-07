using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types.Helpers
{
    public class DirectoryHelper
    {
        private static int CopySubFolders(string src, string dest, bool replaceOlders)
        {
            if (src.ToLower() == dest.ToLower())
                throw new InvalidOperationException("The source and destination path must be different!");

            return CopySubFoldersWithFilesToDestination(src, dest, replaceOlders);
        }

        private static int CopySubFoldersWithFilesToDestination(string sourceFolderPath, string destinationFolderPath, bool replaceOlders)
        {
            if (!Directory.Exists(destinationFolderPath))
                Directory.CreateDirectory(destinationFolderPath);

            int copiedFiles = 0;

            //
            // Get all files and copy each one
            string[] sourceFolderFiles = Directory.GetFiles(sourceFolderPath);
            foreach (string filePath in sourceFolderFiles)
            {
                string filename = Path.GetFileName(filePath);
                string filedest = Path.Combine(destinationFolderPath, filename);

                try
                {
                    File.Copy(filePath, filedest);
                    copiedFiles++;
                }
                catch (IOException)
                {
                    if (replaceOlders)
                    {
                        File.Delete(filedest);
                        File.Copy(filePath, filedest);
                        copiedFiles++;
                    }
                }
            }

            //
            // Get all folders 
            string[] sourceFolders = Directory.GetDirectories(sourceFolderPath);
            foreach (String folderPath in sourceFolders)
            {
                string foldername = Path.GetFileName(folderPath);
                string folderdest = Path.Combine(destinationFolderPath, foldername);

                //
                // Recursive call
                copiedFiles += CopySubFoldersWithFilesToDestination(folderPath, folderdest, replaceOlders);
            }

            return copiedFiles;
        }

    }
}
