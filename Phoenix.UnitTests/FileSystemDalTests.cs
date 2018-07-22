using Phoenix.FileSystemDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Phoenix.UnitTests
{
    public class FileSystemDalTests
    {
        private IFileSystemDal FsDal { get; set; }

        private readonly string CurrentDirectory;
        
        /// <summary>
        /// Note that these tests will actually create files in your directory structure, so make sure to delete 
        /// </summary>
        public FileSystemDalTests()
        {
            this.FsDal = new FileSystemDal.FileSystemDal();

            this.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        [Fact]
        public void FileReadWriteTests()
        {
            var content = "This is some random content that will be written to a file.";

            var fullPath = System.IO.Path.Combine(this.CurrentDirectory, @"Test\Directory\Structure\test.txt");

            this.FsDal.WriteToFile(fullPath, content);

            this.FsDal.TryGetFileContents(fullPath, out string retrievedContents);

            Assert.Equal(content, retrievedContents);
        }
    }
}
