using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Components
{
    public interface ICopyableComponent: IComponent
    {
        IComponent Copy();
    }
}
