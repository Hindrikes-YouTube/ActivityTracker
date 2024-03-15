using ActivityTracker.Shared.Services;
using Shiny.Locations;


namespace ActivityTracker.Services;

public class LocationService : ILocationService
{
    private readonly IGpsManager gpsManager;
    private IDisposable? subscription;
     

    public LocationService(IGpsManager gpsManager)
    {
        this.gpsManager = gpsManager;
    }

    public Task StartListener()
    {
        if (gpsManager.CurrentListener is not null)
            return Task.CompletedTask;

        return gpsManager.StartListener(new GpsRequest
        {
            BackgroundMode = GpsBackgroundMode.Realtime
        });
    }

    public Task StopListener()
    {        
        DisposeSubscription(); 
        return gpsManager.StopListener();
    }

    public void CreateSubscription(Action<Shared.Models.Location> callback)
    {
        subscription = gpsManager.WhenReading().Subscribe(reading => callback.Invoke(new() { Latitude = reading.Position.Latitude, Longitude = reading.Position.Longitude}));
    }

    public void DisposeSubscription()
    {
        subscription?.Dispose();
    }
}

public partial class ActivityTrackerGpsDelegate : IGpsDelegate
{
    private readonly IActivityService activityService;

    public ActivityTrackerGpsDelegate(IActivityService activityService)
    {
        this.activityService = activityService;
    }

    public Task OnReading(GpsReading reading)
    {
        var position = reading.Position;

        return activityService.Log(position.Latitude, position.Longitude, DateTime.Now);
    }
}
