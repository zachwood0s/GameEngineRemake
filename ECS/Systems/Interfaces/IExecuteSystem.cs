using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems.Interfaces
{
    public interface IExecuteSystem: ISystem
    {
        void Execute();
    }
}
