using ActivityTracker.Shared.Models;

namespace ActivityTracker.Shared.Services;

public interface IActivityService
{
    Task<string> Start(string typeOfActivityId);
    Task Pause();
    Task Resume();
    Task Stop();
    Task<CurrentActivitySummary> Log(string activityId, double latitude, double longitude, DateTime time);

}
