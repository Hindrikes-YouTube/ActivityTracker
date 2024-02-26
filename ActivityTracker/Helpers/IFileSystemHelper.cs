using ActivityTracker.Shared.Helpers;

namespace ActivityTracker.Helpers
{
    public class FileSystemHelper : IFileSystemHelper
    {
        public string GetAppDataDirectory()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}
