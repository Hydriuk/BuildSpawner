using BuildSpawner.API;
using Rocket.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.RocketMod.Services
{
    public class ThreadAdapter : IThreadAdapter
    {
        public void RunOnMainThread(Action action) => TaskDispatcher.QueueOnMainThread(action);
    }
}
