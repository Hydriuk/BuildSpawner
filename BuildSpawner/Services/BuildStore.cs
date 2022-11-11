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
        private readonly ILiteCollection<NameRef> _buildNames;

        public BuildStore(IEnvironmentProvider environmentProvider)
        {
            _database = new LiteDatabase(Path.Combine(environmentProvider.PluginDirectory, "builds.db"));

            _builds = _database.GetCollection<BuildModel>("Builds");
            _builds.EnsureIndex(build => build.Name);

            _buildNames = _database.GetCollection<NameRef>("BuildNames");
            _buildNames.EnsureIndex(name => name.Name);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public IEnumerable<string> GetBuildNames()
        {
            return _buildNames.FindAll().Select(name => name.Name);
        }

        public BuildModel GetBuild(string buildName)
        {
            return _builds.FindOne(build => build.Name == buildName);
        }

        public void SaveBuild(BuildModel buildModel)
        {
            _builds.Upsert(buildModel);

            _buildNames.Upsert(new NameRef(buildModel.Name));
        }

        public bool RemoveBuild(string buildName)
        {
            return _builds.DeleteMany(build => build.Name == buildName) >= 1  && 
                _buildNames.DeleteMany(name => name.Name == buildName) >= 1;
        }

        private class NameRef
        {
            [BsonId]
            public string Name { get; set; }

            public NameRef(string name)
            {
                Name = name;
            }
        }
    }
}