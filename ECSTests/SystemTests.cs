using ECS;
using ECS.Components;
using ECS.Systems;
using ECS.Systems.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            _scene.SystemPool.Register(new TestInitSystem(_scene));
            _scene.Initialize();

            Assert.AreEqual(1, _scene.SystemPool.InitializeSystems.Count);

            TestInitSystem sys = _scene.SystemPool.GetSystem<TestInitSystem>();

            Assert.IsTrue(sys.didRun);


        }

        [TestMethod]
        public void SystemPoolExecuteSystems()
        {
            _scene.SystemPool.Register(new TestExecuteSystem());
            _scene.Execute();

            Assert.AreEqual(1, _scene.SystemPool.ExecuteSystems.Count);

            TestExecuteSystem sys = _scene.SystemPool.GetSystem<TestExecuteSystem>();

            Assert.IsTrue(sys.didExecute);
        }

        [TestMethod]
        public void SystemPoolMixedSystems()
        {
            _scene.SystemPool.Register(new TestExecuteInitSystem())
                             .Register(new TestExecuteSystem())
                             .Register(new TestInitSystem(_scene));
            Assert.AreEqual(2, _scene.SystemPool.InitializeSystems.Count);
            Assert.AreEqual(2, _scene.SystemPool.ExecuteSystems.Count);
            _scene.Initialize();

            TestExecuteInitSystem sys = _scene.SystemPool.GetSystem<TestExecuteInitSystem>();
            Assert.IsTrue(sys.didInit);

            _scene.Execute();

            Assert.IsTrue(sys.didExec);
        }

        [TestMethod]
        public void SystemPoolReactiveSystems()
        {
            _scene.SystemPool.Register(new TestPositionSystem(_scene));
            _scene.CreateEntity().With<TestPositionComponent>();

            Assert.AreEqual(1, _scene.SystemPool.ExecuteSystems.Count);

            TestPositionSystem sys = _scene.SystemPool.GetSystem<TestPositionSystem>();

            _scene.Execute();

            Assert.IsTrue(sys.didExecute);

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
        public void Execute()
        {
            didExecute = true;
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
}
