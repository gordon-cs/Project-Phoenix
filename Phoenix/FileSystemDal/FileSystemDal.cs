using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Phoenix.FileSystemDal
{
    public class FileSystemDal : IFileSystemDal
    {
        private const string BaseDirectory = @"\Content";

        private bool IsValidFileNamePath(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
            {
                return false;
            }

            var fileName = Path.GetFileName(fullPath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var directoryName = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrWhiteSpace(directoryName))
            {
                return false;
            }

            return true;
        }


        public bool TryGetFileContents(string fullPath, out string fileContents)
        {
            var isValidPath = this.IsValidFileNamePath(fullPath);

            if(!isValidPath)
            {
                fileContents = null;

                return false;
            }

            if (!System.IO.File.Exists(fullPath))
            {
                fileContents = null;

                return false;
            }
            
            fileContents = System.IO.File.ReadAllText(fullPath);

            return true;
            
        }

        public void WriteToFile(string fullPath, string fileContents)
        {
            var isValidPath = this.IsValidFileNamePath(fullPath);

            if(!isValidPath)
            {
                throw new Exception($"Invalid file path {fullPath}");
            }

            // Make sure the directory exists.
            var directory = System.IO.Path.GetDirectoryName(fullPath);

            System.IO.Directory.CreateDirectory(directory);

            // Now that we are sure the directory exists, write to the file.
            // The following method with create a file and write to it if a file did not exist, or overwrite an existing file.
            System.IO.File.WriteAllText(fullPath, fileContents);
        }
    }
}