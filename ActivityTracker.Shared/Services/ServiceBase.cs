using ActivityTracker.Shared.Helpers;
using LiteDB;

namespace ActivityTracker.Shared.Services;

public abstract class ServiceBase : IDisposable
{
    private static LiteDatabase? database;
    protected LiteDatabase Database => GetDatabase();

    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly IFileSystemHelper fileSystemHelper;

    public ServiceBase(IFileSystemHelper fileSystemHelper)
    {
        this.fileSystemHelper = fileSystemHelper;
    }

    private LiteDatabase GetDatabase()
    {
        semaphoreSlim.Wait();

        if (database == null)
        {
            var path = Path.Combine(fileSystemHelper.GetAppDataDirectory(), "activitytracker.db");

            database = new LiteDatabase(path);
        }

        semaphoreSlim.Release();

        return database;
    }

    protected void DeleteDatabase()
    {
        semaphoreSlim.Wait();

        var path = Path.Combine(fileSystemHelper.GetAppDataDirectory(), "activitytracker.db");

        File.Delete(path);

        semaphoreSlim.Release();
    }

    public void Dispose()
    {
        database?.Dispose();
        database = null;
    }
}
