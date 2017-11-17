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
            _scene.SystemPool.Register(new TestInitSystem(_scene));

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
            _scene.Initialize();

            Assert.AreEqual(1, _scene.SystemPool.InitializeSystems.Count);

            TestInitSystem sys = _scene.SystemPool.GetSystem<TestInitSystem>();

            Assert.IsTrue(sys.didRun);


        }

    }

    public class TestPositionSystem : ReactiveSystem
    {
        public List<Entity> updatedEntities = new List<Entity>(); //merely for testing purposes
        public TestPositionSystem(Scene scene) : base(scene)
        {
        }

        protected override Matcher GetMatcher()
        {
            return new Matcher().Of<TestPositionComponent>();
        }
        public override void Execute(IEnumerable<Entity> entities)
        {
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
