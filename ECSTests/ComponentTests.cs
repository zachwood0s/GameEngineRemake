using ECS;
using ECS.Attributes;
using ECS.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests
{
    [TestClass]
    public class ComponentTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentPool.RegisterAllComponents();
        }

        [TestMethod]
        public void NonSettableComponentTest()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group testGroup = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());
            Watcher watcher = new Watcher(testGroup);

            

            Assert.AreEqual(0, watcher.EntityCount);

            TestPositionComponent comp = test.GetComponent<TestPositionComponent>();
            comp.X = 200;
            comp.Y = 140;

            Assert.AreEqual(0, watcher.EntityCount);

            test.UpdateComponent(comp);

            Assert.AreEqual(1, watcher.EntityCount);
        }

        [TestMethod]
        public void RemoveNonSettableComponentTest()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group testGroup = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());
            Watcher watcher = new Watcher(testGroup);

            Assert.AreEqual(0, watcher.EntityCount);

            TestPositionComponent comp = test.GetComponent<TestPositionComponent>();
            comp.X = 200;
            comp.Y = 140;

            Assert.AreEqual(0, watcher.EntityCount);

            test.UpdateComponent(comp);

            Assert.AreEqual(1, watcher.EntityCount);

            watcher.ClearObservedEntities();

            Assert.AreEqual(0, watcher.EntityCount);

            test.Remove<TestPositionComponent>();

            comp.X = 50;

            test.UpdateComponent(comp);

            Assert.AreEqual(0, watcher.EntityCount);
        }

        [TestMethod]
        public void SettableComponentTest()
        {
            Entity test = _scene.CreateEntity().With<TestSettableComponent>();
            Group testGroup = _scene.GetGroup(new Matcher().Of<TestSettableComponent>());
            Watcher watcher = new Watcher(testGroup);

            Assert.AreEqual(0, watcher.EntityCount);

            TestSettableComponent comp = test.GetComponent<TestSettableComponent>();
            comp.X = 200;
            comp.Y = 140;

            Assert.AreEqual(1, watcher.EntityCount);
        }

        [TestMethod]
        public void RemovingSettableComponentTest()
        {
            Entity test = _scene.CreateEntity().With<TestSettableComponent>();
            Group testGroup = _scene.GetGroup(new Matcher().Of<TestSettableComponent>());
            Watcher watcher = new Watcher(testGroup);

            Assert.AreEqual(0, watcher.EntityCount);

            TestSettableComponent comp = test.GetComponent<TestSettableComponent>();
            comp.X = 200;
            comp.Y = 140;

            Assert.AreEqual(1, watcher.EntityCount);

            watcher.ClearObservedEntities();

            Assert.AreEqual(0, watcher.EntityCount); 

            test.Remove<TestSettableComponent>();

            comp.X = 4000;

            Assert.AreEqual(0, watcher.EntityCount); 
        }


        [Component]
        public class TestSettableComponent: SettableComponent, IComponentHasDefault
        {
            private ComponentProperty<int> _x;
            private ComponentProperty<int> _y;

            public int X
            {
                get => _x.Value;
                set => _x.Value = value; 
            }

            public int Y
            {
                get => _y.Value; 
                set => _y.Value = value; 
            }

            public TestSettableComponent()
            {
                _x = new ComponentProperty<int>(_HandleComponentPropertySet);
                _y = new ComponentProperty<int>(_HandleComponentPropertySet);
            }

            public void SetDefaults()
            {
                X = 100;
                Y = 200;
            }
        }
    }
}
