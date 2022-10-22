#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace BuildSpawner.API
{
#if OPENMOD
    [Service]
#endif
    public interface IThreadAdapter
    {
        void RunOnMainThread(Action action);
    }
}