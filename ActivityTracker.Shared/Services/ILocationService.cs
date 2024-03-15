namespace ActivityTracker.Shared.Services;

public interface ILocationService
{
    Task StartListener();
    Task StopListener();
    void CreateSubscription(Action<Shared.Models.Location> callback);
    void DisposeSubscription();
}

