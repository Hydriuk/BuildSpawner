using BuildSpawner.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSpawner.API
{
    public interface IBuildStore : IDisposable
    {
        IEnumerable<string> GetBuidingsName();
        void SaveBuild(BuildModel buildModel);
        BuildModel GetBuild(string id);
    }
}
