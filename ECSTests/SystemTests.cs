﻿using ECS;
using ECS.Attributes;
using ECS.Components;
using ECS.Systems;
using ECS.Systems.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECSTests
{
    [TestClass]
    public class SystemTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentPool.RegisterAllComponents();

        }

        [TestMethod]
        public void SimpleReactiveSystem()
        {
            TestPositionSystem sys = new TestPositionSystem(_scene);

            Entity test = _scene.CreateEntity().With<TestPositionComponent>();

            sys.WatchList.ClearObservedEntities();
            sys.Execute();

            Assert.AreEqual(0, sys.updatedEntities.Count);

            {
                TestPositionComponent comp = test.GetComponent<TestPositionComponent>();
                comp.X = 15000;
                test.UpdateComponent(comp);
            }

            sys.Execute();

            Assert.AreEqual(1, sys.updatedEntities.Count);
            Assert.AreEqual(15000, sys.updatedEntities[0].GetComponent<TestPositionComponent>().X);
        }

        [TestMethod]
        public void SimpleReactiveSystem2Entities()
        {
            TestPositionSystem sys = new TestPositionSystem(_scene);

            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Entity test2 = _scene.CreateEntity().With<TestPositionComponent>();

            sys.WatchList.ClearObservedEntities();
            sys.Execute();

            Assert.AreEqual(0, sys.updatedEntities.Count);

            {
                TestPositionComponent comp = test.GetComponent<TestPositionComponent>();
                comp.X = 15000;
                test.UpdateComponent(comp);
            }

            {
                TestPositionComponent comp2 = test2.GetComponent<TestPositionComponent>();
                comp2.X = 14000;
                test2.UpdateComponent(comp2);
            }

            sys.Execute();

            Assert.AreEqual(2, sys.updatedEntities.Count);
        }

        [TestMethod]
        public void SystemPoolInitSystems()
        {
            SystemPool sys = new SystemPool();
            sys.Register(new TestInitSystem(_scene));
            _scene.SystemPools.Add(sys);
            _scene.Initialize();

            Assert.AreEqual(1, _scene.SystemPools[0].InitializeSystems.Count);

            TestInitSystem system = _scene.SystemPools[0].GetSystem<TestInitSystem>();

            Assert.IsTrue(system.didRun);


        }

        [TestMethod]
        public void SystemPoolExecuteSystems()
        {

            _scene.SystemPools.Add(new SystemPool().Register(new TestExecuteSystem()));
            _scene.Execute();

            Assert.AreEqual(1, _scene.SystemPools[0].ExecuteSystems.Count);

            TestExecuteSystem sys = _scene.SystemPools[0].GetSystem<TestExecuteSystem>();

            Assert.IsTrue(sys.didExecute);
        }

        [TestMethod]
        public void SystemPoolMixedSystems()
        {
            _scene.SystemPools.Add(new SystemPool().Register(new TestExecuteInitSystem())
                             .Register(new TestExecuteSystem())
                             .Register(new TestInitSystem(_scene)));
            Assert.AreEqual(2, _scene.SystemPools[0].InitializeSystems.Count);
            Assert.AreEqual(2, _scene.SystemPools[0].ExecuteSystems.Count);
            _scene.Initialize();

            TestExecuteInitSystem sys = _scene.SystemPools[0].GetSystem<TestExecuteInitSystem>();
            Assert.IsTrue(sys.didInit);

            _scene.Execute();

            Assert.IsTrue(sys.didExec);
        }

        [TestMethod]
        public void SystemPoolReactiveSystems()
        {
            _scene.SystemPools.Add(new SystemPool().Register(new TestPositionSystem(_scene)));
            _scene.CreateEntity().With<TestPositionComponent>();

            Assert.AreEqual(1, _scene.SystemPools[0].ExecuteSystems.Count);

            TestPositionSystem sys = _scene.SystemPools[0].GetSystem<TestPositionSystem>();

            _scene.Execute();

            Assert.IsTrue(sys.didExecute);
        }

        [TestMethod]
        public void ThreadedSystemPool()
        {
            ThreadedSystemPool pool = new ThreadedSystemPool();
            pool.Register(new TestExecuteSystem());

            pool.Execute();

            Thread.Sleep(5000);

            pool.CleanUp();

            Debug.WriteLine(pool.AvgFps);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().didExecute);
            Assert.IsFalse(pool.IsRunning);
        }

        [TestMethod]
        public void ThreadedSystemPoolLockedFps()
        {
            ThreadedSystemPool pool = new ThreadedSystemPool(60);
            pool.Register(new TestExecuteSystem());

            pool.Execute();

            Thread.Sleep(5000);

            pool.CleanUp();

            Debug.WriteLine(pool.AvgFps);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().didExecute);
            Assert.IsFalse(pool.IsRunning);
        }
        [TestMethod]
        public void ThreadedSystemMultipleSystems()
        {
            ThreadedSystemPool pool = new ThreadedSystemPool(60);
            pool.Register(new TestExecuteSystem());
            ThreadedSystemPool pool2 = new ThreadedSystemPool();
            pool2.Register(new TestExecuteInitSystem());

            _scene.SystemPools.Add(pool);
            _scene.SystemPools.Add(pool2);

            _scene.Initialize();

            _scene.Execute();

            Thread.Sleep(5000);

            _scene.CleanUp();

            Debug.WriteLine(pool.AvgFps);
            Debug.WriteLine(pool2.AvgFps);

            Debug.WriteLine(pool.GetSystem<TestExecuteSystem>().sum);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().didExecute);
            Assert.IsTrue(pool2.GetSystem<TestExecuteInitSystem>().didExec);
            Assert.IsTrue(pool2.GetSystem<TestExecuteInitSystem>().didInit);
            Assert.IsFalse(pool.IsRunning);
            Assert.IsFalse(pool2.IsRunning);
        }

        [TestMethod]
        public void ThreadedReactiveSystems()
        {
            ThreadedSystemPool update = new ECS.Systems.ThreadedSystemPool(60);
            update.Register(new TestTransformSystem(_scene));
            Assert.AreEqual(1, update.ExecuteSystems.Count);

            ThreadedSystemPool render = new ECS.Systems.ThreadedSystemPool(120);
            render.Register(new TestRenderSystem(_scene));
            Assert.AreEqual(1, render.ExecuteSystems.Count);

            ThreadedSystemPool physics = new ECS.Systems.ThreadedSystemPool(80);
            physics.Register(new TestRecoilSystem(_scene));
            Assert.AreEqual(1, physics.ExecuteSystems.Count);

            _scene.SystemPools.Add(update);
            _scene.SystemPools.Add(render);
            _scene.SystemPools.Add(physics);

            Entity testEntity = _scene.CreateEntity().With<PositionComponent>();

            _scene.Initialize();

            _scene.Execute();

            Thread.Sleep(5000);

            _scene.CleanUp();

            Debug.WriteLine(update.AvgFps);
            Debug.WriteLine(render.AvgFps);
        }
        

    }

    public class TestExecuteInitSystem : IExecuteSystem, IInitializeSystem
    {
        public bool didInit = false;
        public bool didExec = false;
        public void Execute()
        {
            didExec = true;
        }

        public void Initialize()
        {
            didInit = true;
        }
    }
    public class TestExecuteSystem : IExecuteSystem
    {
        public bool didExecute = false;
        public double sum = 0;
        public void Execute()
        {
            didExecute = true;
            sum += Math.Sqrt(2508);
            //Debug.WriteLine(sum);
        }
    }
    public class TestPositionSystem : ReactiveSystem
    {
        public List<Entity> updatedEntities = new List<Entity>(); //merely for testing purposes
        public bool didExecute = false;
        public TestPositionSystem(Scene scene) : base(scene)
        {
        }

        protected override Matcher GetMatcher()
        {
            return new Matcher().Of<TestPositionComponent>();
        }
        public override void Execute(IEnumerable<Entity> entities)
        {
            didExecute = true;
            foreach(Entity e in entities)
            {
                updatedEntities.Add(e); 
            }
        }
    }


    public class TestInitSystem: IInitializeSystem
    {
        public bool didRun = false;
        private Scene _scene;
        public TestInitSystem(Scene scene)
        {
            _scene = scene;
        }
        public void Initialize()
        {
            didRun = true;
        }
    }



    /*public class TestRenderSystem: IExecuteSystem
    {
        private Scene _scene;
        private Group _group;
        public TestRenderSystem(Scene scene)
        {
            _scene = scene;
            _group = _scene.GetGroup(new Matcher().Of<PositionComponent>());
        }
        public void Execute()
        {
            foreach(Entity entity in _group)
            {
                PositionComponent comp = entity.GetComponent<PositionComponent>();
                Debug.WriteLine("{0}, {1}", comp.X, comp.Y);
            }
        }
    }*/
    public class TestRenderSystem: GroupExecuteSystem
    {
        public TestRenderSystem(Scene scene) : base(scene) { }
        public override Matcher GetMatcher() => new Matcher().Of<PositionComponent>();
        public override void Execute(Entity entity)
        {
            PositionComponent comp = entity.GetComponent<PositionComponent>();
            //Debug.WriteLine("{0}, {1}", comp.X, comp.Y);
        }
    }
    public class TestRecoilSystem: GroupExecuteSystem
    {
        public TestRecoilSystem(Scene scene) : base(scene) { }
        public override Matcher GetMatcher() => new Matcher().Of<PositionComponent>();
        public override void Execute(Entity entity)
        {
            PositionComponent comp = entity.GetComponent<PositionComponent>();
            int newX = comp.X - 100;
            entity.UpdateComponent(new PositionComponent(newX, comp.Y));
        }
    }
    /*public class TestRecoilSystem : IExecuteSystem
    {
        private Scene _scene;
        private Group _group;
        public TestRecoilSystem(Scene scene)
        {
            _scene = scene;
            _group = _scene.GetGroup(new Matcher().Of<PositionComponent>());
        }
        public void Execute()
        {
            foreach(Entity entity in _group)
            {
                PositionComponent comp = entity.GetComponent<PositionComponent>();
                int newX = comp.X - 100;
                entity.UpdateComponent(new PositionComponent(newX, comp.Y));
            }
        }
    }*/
    public class TestTransformSystem : ThreadSafeReactiveSystem
    {
        public TestTransformSystem(Scene scene) : base(scene)
        {
        }
        protected override Matcher GetMatcher()
        {
            return new Matcher().Of<PositionComponent>();
        }
        public override void Execute(Entity entity)
        {
            PositionComponent comp = entity.GetComponent<PositionComponent>();
            int newX = comp.X + 1;
            int newY = comp.Y + 1;
            entity.UpdateComponent(new PositionComponent(newX, newY));
        }
    }
    [Component]
    public class PositionComponent: IComponentHasDefault
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public PositionComponent() { }
        public PositionComponent(int x, int y)
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
