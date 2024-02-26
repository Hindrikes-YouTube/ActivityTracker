using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.Shared.Models;

public class CurrentActivitySummary
{
    public double Distance { get; set; }

    public CurrentActivitySummary(double distance,TimeSpan duration)
    {
        Distance = distance;
        Duration = duration;
    }

    public TimeSpan Duration { get; set; }
}
