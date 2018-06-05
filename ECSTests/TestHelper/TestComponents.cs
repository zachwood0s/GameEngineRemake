using ECS.Attributes;
using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests.TestHelper
{
    [Component]
    class TestComponent1 : IComponentHasDefault
    {
        public int X { get; set; }
        public int Y { get; set; }
        public void SetDefaults()
        {
            X = 0;
            Y = 0;
        }
    }

    [Component]
    class TestComponent2 : IComponentHasDefault
    {
        public int Z { get; set; }
        public int W { get; set; }
        public void SetDefaults()
        {
            Z = 0;
            W = 0;
        }
    }

    [Component]
    class UnregisteredComponent : IComponentHasDefault
    {
        public int Z { get; set; }
        public int W { get; set; }
        public void SetDefaults()
        {
            Z = 0;
            W = 0;
        }
    }
}
