using BuildSpawner.API;
using BuildSpawner.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSpawner.Services
{
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
