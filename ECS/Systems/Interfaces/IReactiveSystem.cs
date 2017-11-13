using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems.Interfaces
{
    interface IReactiveSystem: IExecuteSystem
    {
        Watcher WatchList
        {
            get;
        }

        void Clear();
        void Disable();
        void Enable();

    }
}
