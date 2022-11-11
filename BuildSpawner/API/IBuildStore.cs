using BuildSpawner.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;

namespace BuildSpawner.API
{
#if OPENMOD
    [Service]
#endif
    public interface IBuildStore : IDisposable
    {
        IEnumerable<string> GetBuildNames();
        void SaveBuild(BuildModel buildModel);
        BuildModel GetBuild(string buildName);
        bool RemoveBuild(string buildName);
    }
}