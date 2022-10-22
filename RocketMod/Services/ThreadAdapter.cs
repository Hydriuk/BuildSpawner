using BuildSpawner.API;
using Rocket.Core.Utils;
using System;

namespace BuildSpawner.RocketMod.Services
{
    public class ThreadAdapter : IThreadAdapter
    {
        public void RunOnMainThread(Action action) => TaskDispatcher.QueueOnMainThread(action);
    }
}