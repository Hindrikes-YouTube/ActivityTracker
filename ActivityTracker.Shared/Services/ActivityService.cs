using ActivityTracker.Shared.Entities;
using ActivityTracker.Shared.Helpers;
using ActivityTracker.Shared.Models;

namespace ActivityTracker.Shared.Services;

public class ActivityService : ServiceBase, IActivityService
{
    private double distance;
    private TimeSpan duration;
    private string? currentActivityId;

    public ActivityService(IFileSystemHelper fileSystemHelper) : base(fileSystemHelper)
    {
    }

    public async Task<string> Start(string typeOfActivityId)
    {
        var collection = Database.GetCollection<Activity>();
        var activity = new Activity()
        {
            Id = Guid.NewGuid().ToString(),
            TypeOfActivityId = typeOfActivityId,
            Events = [new() { Time = DateTime.Now, TypeOfEvent = ActivityEventType.Start }]
        };

        collection.Insert(activity);

        distance = 0;
        duration = TimeSpan.Zero;

        currentActivityId = activity.Id;

        return activity.Id;
    }

    public Task Pause()
    {
        if (currentActivityId is null)
        {
            return Task.CompletedTask;
        }

        var collection = Database.GetCollection<Activity>();
        var activity = collection.Query().Where(x => x.Id == currentActivityId).First();
        activity.Events.Add(new() { Time = DateTime.Now, TypeOfEvent = ActivityEventType.Pause });

        collection.Update(activity);

        return Task.CompletedTask;
    }

    public Task Resume()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        if (currentActivityId is null)
        {
            return Task.CompletedTask;
        }

        var collection = Database.GetCollection<Activity>();
        var activity = collection.Query().Where(x => x.Id == currentActivityId).First();
        activity.Events.Add(new() { Time = DateTime.Now, TypeOfEvent = ActivityEventType.Stop });
        activity.Duration = duration;
        activity.Distance = distance;

        collection.Update(activity);

        currentActivityId = null;

        return Task.CompletedTask;
    }

    private DateTime lastLog;
    public Task<CurrentActivitySummary> Log(double latitude, double longitude, DateTime time)
    {
        if (currentActivityId is null)
        {
            return Task.FromResult<CurrentActivitySummary>(new(0, TimeSpan.Zero));
        }

        if (lastLog != DateTime.MinValue || time - lastLog < TimeSpan.FromSeconds(1))
        {
            return Task.FromResult<CurrentActivitySummary>(new(distance, duration));
        }

        var collection = Database.GetCollection<Activity>();
        var activity = collection.Query().Where(x => x.Id == currentActivityId).First();
   
        var lastEvent = activity.Events.Last();

        if (lastEvent.TypeOfEvent == ActivityEventType.Pause)
        {
            return Task.FromResult<CurrentActivitySummary>(new(distance, duration));
        }

        var lastLocation = activity.Locations.LastOrDefault();

        var currentLocation = new Entities.Location()
        {
            Latitude = latitude,
            Longitude = longitude,
            Time = time
        };

        activity.Locations.Add(currentLocation);

        //Note if person is moving when paused the distance will be wrong right now.
        if (lastLocation is not null)
        {
            var newDistance = Microsoft.Maui.Devices.Sensors.Location.CalculateDistance(lastLocation.Latitude, lastLocation.Longitude, currentLocation.Latitude, currentLocation.Longitude, DistanceUnits.Kilometers);
            distance += newDistance;
        }

        if (activity.Events.Count == 1)
        {
            duration = time - activity.Events[0].Time;
        }
        else
        {
            duration = CalculateDuration(activity);
        }

        lastLocation = currentLocation;

        collection.Update(activity);

        return Task.FromResult<CurrentActivitySummary>(new(distance, duration));
    }

    private TimeSpan CalculateDuration(Activity activity)
    {
        TimeSpan duration = TimeSpan.Zero;

        DateTime last = DateTime.MinValue;

        foreach (var e in activity.Events)
        {
            if (e.TypeOfEvent != ActivityEventType.Resume)
            {
                var d = e.Time - last;
                duration.Add(d);
            }

            last = e.Time;
        }

        return duration;
    }

    public double GetCurrentDistance() => distance;
    public TimeSpan GetCurrentDuration() => duration;

    public Task<List<ActivityType>> GetActivityTypes()
    {
        return Task.FromResult<List<ActivityType>>([
            new ActivityType("run", "Run"),
            new ActivityType("ride", "Ride"),
            new ActivityType("skiing", "Skiing")
            ]);
    }
}
