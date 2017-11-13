﻿using ECS;
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
    public class WatcherTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentPool.RegisterComponent<TestPositionComponent>();
            ComponentPool.RegisterComponent<TestPositionComponent2>();
        }

        [TestMethod]
        public void SimpleWatcher()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group testPositions = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());
            Watcher positionWatcher = new Watcher(testPositions);

            Assert.AreEqual(0, positionWatcher.EntityCount);

            TestPositionComponent newPos = new TestPositionComponent();
            newPos.X = 10000;
            test.UpdateComponent(newPos);

            Assert.AreEqual(1, positionWatcher.EntityCount);
            Assert.AreEqual(10000, positionWatcher[0].GetComponent<TestPositionComponent>().X);
        }
    }
}
