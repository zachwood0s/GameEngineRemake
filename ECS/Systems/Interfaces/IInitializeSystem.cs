using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems.Interfaces
{
    interface IInitializeSystem: ISystem
    {
        void Initialize();
    }
}
