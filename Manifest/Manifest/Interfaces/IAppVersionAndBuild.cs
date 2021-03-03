using System;
namespace Manifest.Interfaces
{
    public interface IAppVersionAndBuild
    {
        string GetVersionNumber();
        string GetBuildNumber();
    }
}
