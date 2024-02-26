using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.Shared;

public abstract class ActivityTrackerComponentBase : ComponentBase
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;
}
