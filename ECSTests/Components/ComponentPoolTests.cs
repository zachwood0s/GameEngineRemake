using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS.Components;
using ECSTests.TestHelper;

namespace ECSTests.Components
{
    [TestClass]
    public class ComponentPoolTests
    {
        [TestCategory("Generic"), TestMethod]
        public void RegisteringAComponentGeneric()
        {
            ComponentPool.RegisterComponent<TestComponent1>();
            Assert.AreNotEqual(-1, ComponentPool.GetComponentIndex<TestComponent1>());
        }

        [TestCategory("Non-Generic"), TestMethod]
        public void RegisteringAComponentNonGeneric()
        {
            ComponentPool.RegisterComponent(typeof(TestComponent1));
            Assert.AreNotEqual(-1, ComponentPool.GetComponentIndex(typeof(TestComponent1)));
        }

        [TestMethod]
        public void RegisteringAllComponents()
        {
            ComponentPool.RegisterAllComponents();
            //The two are pulled from the test helper class
            //TestComponent1, TestComponent2
            //UnregisteredComponent doesn't happen because it doesn't have the [Component] attribute
            Assert.AreEqual(2, ComponentPool.Components.Count);
        }
    }
}
