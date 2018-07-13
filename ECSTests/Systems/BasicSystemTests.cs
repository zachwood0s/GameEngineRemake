using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;
using ECS.Systems;
using ECSTests.TestHelper;
using System.Threading;
using System.Diagnostics;

namespace ECSTests.Systems
{
    /// <summary>
    /// Summary description for BasicSystemTests
    /// </summary>
    [TestClass]
    public class BasicSystemTests
    {

        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentHelper.RegisterTestComponents();
        }

        #region System Pool

        [TestCategory("Adding to System Pool"), TestMethod]
        public void SystemPoolRegisterExecuteSystem()
        {
            var pool = new SystemPool("pool");
            pool.Register(new TestExecuteSystem());
            Assert.AreEqual(1, pool.ExecuteSystemCount);
        }

        [TestCategory("Adding to System Pool"), TestMethod]
        public void SystemPoolRegisterInitSystem()
        {
            var pool = new SystemPool("pool");
            pool.Register(new TestInitSystem());
            Assert.AreEqual(1, pool.InitializeSystemCount);
        }

        [TestCategory("Adding to System Pool"), TestMethod]
        public void SystemPoolRegisterInitAndExecuteSystem()
        {
            var pool = new SystemPool("pool");
            pool.Register(new TestExecuteInitSystem());
            Assert.AreEqual(1, pool.ExecuteSystemCount);
            Assert.AreEqual(1, pool.InitializeSystemCount);
        }

        [TestCategory("Adding to System Pool"), TestMethod]
        public void SystemPoolMixedSystems()
        {
            var pool = new SystemPool("pool");
            var ei = new TestExecuteInitSystem();
            var e = new TestExecuteSystem();
            var i = new TestInitSystem();
            pool.Register(ei);
            pool.Register(e);
            pool.Register(i);

            Assert.AreEqual(2, pool.ExecuteSystemCount);
            Assert.AreEqual(2, pool.InitializeSystemCount);

            pool.Initialize();
            Assert.IsTrue(ei.DidInit);
            Assert.IsTrue(i.DidInit);

            pool.Execute();
            Assert.IsTrue(ei.DidExecute);
            Assert.IsTrue(e.DidExecute);
        }

        [TestCategory("Get System from System Pool"), TestMethod]
        public void SystemPoolRetrieveSystem()
        {
            var pool = new SystemPool("pool");
            var e = new TestExecuteSystem();
            var i = new TestInitSystem();
            pool.Register(e);
            pool.Register(i);

            Assert.AreEqual(e, pool.GetSystem<TestExecuteSystem>());
            Assert.AreEqual(i, pool.GetSystem<TestInitSystem>());
        }

        #endregion

        #region Execute/Init Systems

        [TestCategory("Execute Systems"), TestMethod]
        public void ExecuteSystemSimple()
        {
            var pool = new SystemPool("pool");
            var sys = new TestExecuteSystem();
            pool.Register(sys);
            pool.Execute();
            Assert.IsTrue(sys.DidExecute);
            Assert.AreEqual(1, sys.RunCount);
        }

        [TestCategory("Init Systems"), TestMethod]
        public void InitSystemSimple()
        {
            var pool = new SystemPool("pool");
            var sys = new TestInitSystem();
            pool.Register(sys);
            pool.Initialize();
            Assert.IsTrue(sys.DidInit);
        }

        #endregion

        #region Reactive Systems

        [TestCategory("Reactive Systems"), TestMethod]
        public void ReactiveSystemSimple()
        {
            var pool = new SystemPool("pool");
            var sys = new TestReactiveSystem(_scene);
            var e = _scene.CreateEntity().With<TestComponent1>();

            pool.Register(sys);
            _scene.AddSystemPool(pool);

           
            _scene.Execute();

            Assert.AreEqual(1, sys.UpdatedEntities.Count);
            sys.UpdatedEntities.Clear();

            _scene.Execute();

            Assert.AreEqual(0, sys.UpdatedEntities.Count);

            e.UpdateComponent((TestComponent1 comp) =>
            {
                comp.X = 15000;
            });

            _scene.Execute();

            Assert.AreEqual(1, sys.UpdatedEntities.Count);
            Assert.AreEqual(15000, sys.UpdatedEntities[0].GetComponent<TestComponent1>().X);
        }

        [TestCategory("Reactive Systems"), TestMethod]
        public void ReactiveSystemTwoEntities()
        {
            var pool = new SystemPool("pool");
            var sys = new TestReactiveSystem(_scene);
            var e1 = _scene.CreateEntity().With<TestComponent1>();
            var e2 = _scene.CreateEntity().With<TestComponent1>();

            pool.Register(sys);
            _scene.AddSystemPool(pool);

            _scene.Execute();

            Assert.AreEqual(2, sys.UpdatedEntities.Count);
            sys.UpdatedEntities.Clear();

            _scene.Execute();

            Assert.AreEqual(0, sys.UpdatedEntities.Count);

            e1.UpdateComponent((TestComponent1 comp) =>
            {
                comp.X = 15000;
            });

            _scene.Execute();

            Assert.AreEqual(1, sys.UpdatedEntities.Count);
            Assert.AreEqual(15000, sys.UpdatedEntities[0].GetComponent<TestComponent1>().X);
        }

        #endregion

        #region Threaded System Pool

        [TestCategory("Threaded System Pool"), TestMethod]
        public void ThreadedSystemPool()
        {
            var pool = new ThreadedSystemPool("pool");
            pool.Register(new TestExecuteSystem());

            pool.Execute();

            Thread.Sleep(1000);

            pool.CleanUp();

            Debug.WriteLine(pool.AvgFps);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().DidExecute);
            Assert.IsFalse(pool.IsRunning);
        }

        [TestCategory("Threaded System Pool"), TestMethod]
        public void ThreadedSystemPoolLockedFps()
        {
            ThreadedSystemPool pool = new ThreadedSystemPool("test",60);
            pool.Register(new TestExecuteSystem());

            pool.Execute();

            Thread.Sleep(2000);

            pool.CleanUp();

            Debug.WriteLine(pool.AvgFps);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().DidExecute);
            Assert.IsFalse(pool.IsRunning);

            //Just to make sure it attempted to limit it.
            //Would be hard to test it against exactly 60 as its not that accurate
            Assert.IsTrue(pool.AvgFps < 120);
        }

        [TestCategory("Threaded System Pool"), TestMethod]
        public void ThreadedSystemPoolMultipleSystemPools()
        {
            ThreadedSystemPool pool = new ThreadedSystemPool("test1", 60);
            pool.Register(new TestExecuteSystem());
            ThreadedSystemPool pool2 = new ThreadedSystemPool("test2");
            pool2.Register(new TestExecuteInitSystem());

            _scene.AddSystemPool(pool);
            _scene.AddSystemPool(pool2);

            _scene.Initialize();

            _scene.Execute();

            Thread.Sleep(2000);

            _scene.CleanUp();

            Debug.WriteLine(pool.AvgFps);
            Debug.WriteLine(pool2.AvgFps);

            Debug.WriteLine(pool.GetSystem<TestExecuteSystem>().RunCount);

            Assert.IsTrue(pool.GetSystem<TestExecuteSystem>().DidExecute);
            Assert.IsTrue(pool2.GetSystem<TestExecuteInitSystem>().DidExecute);
            Assert.IsTrue(pool2.GetSystem<TestExecuteInitSystem>().DidInit);
            Assert.IsFalse(pool.IsRunning);
            Assert.IsFalse(pool2.IsRunning);

            //Should hold true for everything.
            //Limited fps vs unlimited
            Assert.IsTrue(pool.AvgFps < pool2.AvgFps);
        }

        #endregion

    }
}
