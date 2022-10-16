using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.API
{
    public interface IThreadAdapter
    {
        void RunOnMainThread(Action action);
    }
}
