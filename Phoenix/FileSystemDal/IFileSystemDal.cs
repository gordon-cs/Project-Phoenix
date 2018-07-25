namespace Phoenix.FileSystemDal
{
    public interface IFileSystemDal
    {
        void WriteToFile(string fullPath, string fileContents);

        bool TryGetFileContents(string fullPath, out string fileContents);
    }
}