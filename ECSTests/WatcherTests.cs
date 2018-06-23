using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Matching;
using ECSTests.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests
{
    [TestClass]
    public class WatcherTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentPool.RegisterComponent<TestComponent1>();
            ComponentPool.RegisterComponent<TestComponent2>();
        }

        [TestMethod]
        public void SimpleWatcher()
        {
            Entity test = _scene.CreateEntity().With<TestComponent1>();
            Group testPositions = _scene.GetGroup(new Matcher().Of<TestComponent1>());
            Watcher positionWatcher = new Watcher(testPositions);

            Assert.AreEqual(0, positionWatcher.EntityCount);

            TestComponent1 newPos = new TestComponent1
            {
                X = 10000
            };
            test.UpdateComponent(newPos);

            Assert.AreEqual(1, positionWatcher.EntityCount);
            Assert.AreEqual(10000, positionWatcher[0].GetComponent<TestComponent1>().X);
        }
        [TestMethod]
        public void ComponentAdded()
        {
            Entity test = _scene.CreateEntity();

            Group testPositions = _scene.GetGroup(new Matcher().Of<TestComponent1>());
            Watcher positionWatcher = new Watcher(testPositions);

            Assert.AreEqual(0, positionWatcher.EntityCount);

            test.With<TestComponent1>();

            Assert.AreEqual(1, positionWatcher.EntityCount);
        }

        [TestMethod]
        public void ComponentAddedMakesInvalid()
        {
            Entity test = _scene.CreateEntity().With<TestComponent1>();

            Group testPosition = _scene.GetGroup(new Matcher().Of<TestComponent1>().NoneOf(typeof(TestComponent2)));
            Watcher positionWatcher = new Watcher(testPosition);

            Assert.AreEqual(0, positionWatcher.EntityCount);
            test.With<TestComponent2>();
            Assert.AreEqual(0, positionWatcher.EntityCount);

            test.UpdateComponent(new TestComponent1 { X = 100, Y = 300 });

            Assert.AreEqual(0, positionWatcher.EntityCount);
        }
    }
}
