using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests.TestHelper
{
    public static class EntityFactory
    {
        public static EntityChangedEventHandler EmptyEntityChangedHandler = (ent, ind, comp) => { };

        public static Entity EntityWithNoComponents(Scene scene)
        {
            return scene.CreateEntity();
        }
        public static Entity EntityWithOneComponent(Scene scene)
        {
            return scene.CreateEntity().With<TestComponent1>();
        }
        public static Entity EntityWithTwoComponents(Scene scene)
        {
            return scene.CreateEntity().With<TestComponent1>().With<TestComponent2>();
        }
    }
}
