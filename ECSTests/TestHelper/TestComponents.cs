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
    class TestComponent1 : IComponentHasDefault, ICopyableComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public void SetDefaults()
        {
            X = 0;
            Y = 0;
        }
        public IComponent Copy()
        {
            return new TestComponent1() { X = X, Y = Y };
        }
    }

    [Component]
    class TestComponent2 : IComponentHasDefault, ICopyableComponent
    {
        public int Z { get; set; }
        public int W { get; set; }
        public void SetDefaults()
        {
            Z = 0;
            W = 0;
        }
        public IComponent Copy()
        {
            return new TestComponent2() { Z = Z, W = W };
        }
    }

    class UnregisteredComponent : IComponentHasDefault, ICopyableComponent
    {
        public int Z { get; set; }
        public int W { get; set; }
        public void SetDefaults()
        {
            Z = 0;
            W = 0;
        }
        public IComponent Copy()
        {
            return new TestComponent2() { Z = Z, W = W };
        }
    }
}
