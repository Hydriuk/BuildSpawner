using BuildSpawner.API;
using BuildSpawner.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuildSpawner.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class BuildStore : IBuildStore
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<BuildModel> _builds;

        public BuildStore()
        {
            _database = new LiteDatabase("builds.db");

            _builds = _database.GetCollection<BuildModel>("Builds");
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public IEnumerable<string> GetBuidingsName()
        {
            return _builds.FindAll().Select(build => build.Id);
        }

        public BuildModel GetBuild(string id)
        {
            return _builds.FindOne(build => build.Id == id);
        }

        public void SaveBuild(BuildModel buildModel)
        {
            _builds.Upsert(buildModel);
        }
    }
}