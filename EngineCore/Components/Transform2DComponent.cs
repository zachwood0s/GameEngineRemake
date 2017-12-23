using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    class Transform2DComponent : IComponentHasDefault
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Transform2DComponent() { }
        public Transform2DComponent(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void SetDefaults()
        {
            X = 0;
            Y = 0;
        }
    }
}
