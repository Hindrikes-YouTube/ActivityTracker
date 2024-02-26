
using ActivityTracker.Shared.Entities;
using ActivityTracker.Shared.Helpers;
using ActivityTracker.Shared.Services;
using FluentAssertions;
using LiteDB;
using System.Net.Http.Headers;

namespace ActivityTracker.UnitTests;

public class ActivityServiceTests : IAsyncLifetime
{
    private IFileSystemHelper fileSystemHelper;

    public ActivityServiceTests()
    {
        fileSystemHelper = new TestFileSystemHelper();
    }

    public Task DisposeAsync()
    {
        var dbPath = Path.Combine(fileSystemHelper.GetAppDataDirectory(), "activitytracker.db");

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
       return Task.CompletedTask;
    }

    [Fact]
    public async Task StartActivityTest()
    {
        var service = new ActivityService(fileSystemHelper);
        var id = await service.Start("run");

        id.Should().NotBeNullOrWhiteSpace();

        service.Dispose();

        var activity = GetActivity(id);

        activity.Events.Should().HaveCount(1);
    }

    [Fact]
    public async Task LogActivityTest()
    {
        var id = Guid.NewGuid().ToString();

        var startTime = DateTime.Now.AddSeconds(-5);

        AddActivity(new Activity()
        {
            Id = id,
            TypeOfActivityId = "run",
            Events = [new() { Time = startTime,TypeOfEvent = ActivityEventType.Start}],
            Locations = [new() { Latitude = 60.403604, Longitude = 15.463210, Time = startTime }]
        });

        var service = new ActivityService(fileSystemHelper);
        var result = await service.Log(id, 60.403016, 15.463499, startTime.AddSeconds(5));

        result.Duration.Seconds.Should().Be(5);
        Math.Round(result.Distance, 3).Should().Be(0.067);

        service.Dispose();
    }

    private Activity GetActivity(string id)
    {
        var path = Path.Combine(fileSystemHelper.GetAppDataDirectory(), "activitytracker.db");

        var database = new LiteDatabase(path);

        var activity =  database.GetCollection<Activity>().Query().Where(x => x.Id == id).First();

        database.Dispose();

        return activity;
    }

    private void AddActivity(Activity activity)
    {
        var path = Path.Combine(fileSystemHelper.GetAppDataDirectory(), "activitytracker.db");

        var database = new LiteDatabase(path);
        
        database.GetCollection<Activity>().Insert(activity);

        database.Dispose();
    }
}