using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS.Components;
using ECS;

namespace ECSTests
{
    [TestClass]
    public class EntityTests
    {
        private Scene _scene;
        private Entity _testEntity;

        [TestInitialize]
        public void Initialize()
        {
            ComponentPool.RegisterComponent<TestPositionComponent>();
            ComponentPool.RegisterComponent<TestPositionComponent2>();

            _scene = new Scene();
            _testEntity = _scene.CreateEntity();
        } 

        [TestCleanup]
        public void Cleanup()
        {
            
        }

        #region Adding Components
        [TestMethod]
        public void AddComponentGeneric()
        {
            _testEntity.With<TestPositionComponent>();
            Assert.AreEqual(1, _testEntity.Components.Count);
        }

        [TestMethod]
        public void AddComponentNonGeneric()
        {
            TestPositionComponent comp = new TestPositionComponent();
            _testEntity.With(comp);
            Assert.AreEqual(1, _testEntity.Components.Count);
        }

        [TestMethod]
        public void AddTwoComponents()
        {
            _testEntity.With<TestPositionComponent>()
                       .With<TestPositionComponent2>();
            Assert.AreEqual(2, _testEntity.Components.Count);
        }

        [TestMethod]
        public void IgnoreDuplicateComponents()
        {
            _testEntity.With<TestPositionComponent>()
                       .With<TestPositionComponent>();
            Assert.AreEqual(1, _testEntity.Components.Count);
        }
        #endregion

        #region Removing Components
        
        [TestMethod]
        public void RemoveComponentGeneric()
        {
            _testEntity.With<TestPositionComponent>();
            _testEntity.Remove<TestPositionComponent>();
            Assert.AreEqual(0, _testEntity.Components.Count);
        }


        [TestMethod]
        public void RemoveComponentNonGeneric()
        {
            TestPositionComponent comp = new TestPositionComponent();
            _testEntity.With(comp);
            _testEntity.Remove(comp);
            Assert.AreEqual(0, _testEntity.Components.Count);
        }

        [TestMethod]
        public void RemoveComponentByType()
        {
            TestPositionComponent comp = new TestPositionComponent();
            _testEntity.With(comp);
            _testEntity.Remove(typeof(TestPositionComponent));
            Assert.AreEqual(0, _testEntity.Components.Count);
        }

        [TestMethod]
        public void RemoveTwoComponents()
        {
            _testEntity.With<TestPositionComponent>()
                       .With<TestPositionComponent2>();
            _testEntity.Remove<TestPositionComponent>()
                       .Remove<TestPositionComponent2>();
            Assert.AreEqual(0, _testEntity.Components.Count);
        }
        #endregion

        #region Getting/Checking/Updating Components
        
        [TestMethod]
        public void GetComponentsWithDefaults()
        {
            SetupBasicEntity();

            TestPositionComponent comp = _testEntity.GetComponent<TestPositionComponent>();
            Assert.IsNotNull(comp);
            Assert.AreEqual(0, comp.X);
            Assert.AreEqual(0, comp.Y);

            TestPositionComponent2 comp2 = _testEntity.GetComponent<TestPositionComponent2>();
            Assert.IsNotNull(comp2);
            Assert.AreEqual(10, comp2.X);
            Assert.AreEqual(14, comp2.Y);
        } 

        [TestMethod]
        public void GetComponentsWithoutDefaults()
        {
            TestPositionComponent comp = new TestPositionComponent();
            comp.X = 10;
            comp.Y = 100;
            _testEntity.With(comp);

            TestPositionComponent getComp = _testEntity.GetComponent<TestPositionComponent>();
            Assert.IsNotNull(getComp);
            Assert.AreEqual(10, getComp.X);
            Assert.AreEqual(100, getComp.Y);
        }

        [TestMethod]
        public void GetComponentNotValid()
        {
            Assert.IsNull(_testEntity.GetComponent<TestPositionComponent>());
        }

        [TestMethod]
        public void HasComponent()
        {
            SetupBasicEntity();

            Assert.IsTrue(_testEntity.HasComponent<TestPositionComponent>());
            Assert.IsTrue(_testEntity.HasComponent<TestPositionComponent2>());
        }

        [TestMethod]
        public void HasComponentNotValid()
        {
            Assert.IsFalse(_testEntity.HasComponent<TestPositionComponent>());
        }

        #endregion


        public void SetupBasicEntity()
        {
            _testEntity.With<TestPositionComponent>()
                       .With<TestPositionComponent2>();
        }
    }


    class TestPositionComponent2 : IComponentHasDefault
    {
        public int X { get; set; } 
        public int Y { get; set; }
        public void SetDefaults()
        {
            X = 10;
            Y = 14;
        }
    }
    class TestPositionComponent : IComponentHasDefault
    {
        public int X { get; set; } 
        public int Y { get; set; }
        public void SetDefaults()
        {
            X = 0;
            Y = 0;
        }
    }
}
