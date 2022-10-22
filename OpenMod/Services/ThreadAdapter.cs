using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using System;

namespace BuildSpawner.OpenMod.Services
{
    [PluginServiceImplementation]
    public class ThreadAdapter : IThreadAdapter
    {
        public async void RunOnMainThread(Action action)
        {
            await UniTask.SwitchToMainThread();

            action();
        }
    }
}