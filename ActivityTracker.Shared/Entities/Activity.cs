namespace ActivityTracker.Shared.Entities;

public class Activity
{
    public required string Id { get; init; }
    public required string TypeOfActivityId { get; init; }
    public List<ActivityEvent> Events { get; set; } = new();
    public List<Location> Locations { get; set; } = new();

    public double? Distance { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Comment { get; set; }
}

public record ActivityEvent
{
    public ActivityEventType TypeOfEvent { get; set; }
    public DateTime Time { get; set; }
}

public record Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Time { get; set; }
}

public enum ActivityEventType
{
    Start,
    Pause,
    Resume,
    Stop
}
