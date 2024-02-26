using ActivityTracker.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.UnitTests;

public class TestFileSystemHelper : IFileSystemHelper
{
    public string GetAppDataDirectory()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
}
