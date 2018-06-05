using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests.TestHelper
{
    public static class ComponentHelper
    {
        public static void RegisterTestComponents()
        {
            ComponentPool.RegisterComponent<TestComponent1>();
            ComponentPool.RegisterComponent<TestComponent2>();
        }
    }
}
