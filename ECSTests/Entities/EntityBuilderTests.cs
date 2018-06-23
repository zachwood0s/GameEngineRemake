using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;
using ECSTests.TestHelper;
using ECS.Entities;

namespace ECSTests.Entities
{
    [TestClass]
    public class EntityBuilderTests
    {

        private Scene _scene;

        [TestInitialize]
        public void Initialize()
        {
            ComponentHelper.RegisterTestComponents();
            _scene = new Scene();
        }

        #region Adding Components

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderPlainEntity()
        {
            var e = new EntityBuilder().Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(0, e.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityOneComponent()
        {
            var e = new EntityBuilder().With<TestComponent1>().Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(1, e.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityTwoComponents()
        {
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .With<TestComponent2>()
                      .Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(2, e.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityDuplicateComponent()
        {
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .With<TestComponent1>()
                      .Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(1, e.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityOneComponentCreationFunction()
        {
            var compX = 7;
            var e = new EntityBuilder()
                      .With(() => new TestComponent1() { X = compX, Y = 0 })
                      .Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(1, e.Components.Count);
            var getComp = e.GetComponent<TestComponent1>();
            Assert.AreEqual(compX, getComp.X);
        }

        #endregion

        #region Notifying Groups

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderUpdatingGroupsOneComponent()
        {
            var g = _scene.GetGroup(new ECS.Matching.Matcher().Of<TestComponent1>());
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .Build(_scene);
            Assert.AreEqual(1, g.EntityCount);
        }

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderUpdatingGroupsOneInvalidComponent()
        {
            var g = _scene.GetGroup(new ECS.Matching.Matcher().Of<TestComponent2>());
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .Build(_scene);
            Assert.AreEqual(0, g.EntityCount);
        }

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderUpdatingGroupsTwoComponents()
        {
            var g = _scene.GetGroup(new ECS.Matching.Matcher()
                          .AllOf(typeof(TestComponent1), typeof(TestComponent2)));
               
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .With<TestComponent2>()
                      .Build(_scene);
            Assert.AreEqual(1, g.EntityCount);
        }

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderGetGroupAfterCreation()
        {
            var e = new EntityBuilder()
                      .With<TestComponent1>()
                      .With<TestComponent2>()
                      .Build(_scene);

            var g = _scene.GetGroup(new ECS.Matching.Matcher()
                          .AllOf(typeof(TestComponent1), typeof(TestComponent2)));

            Assert.AreEqual(1, g.EntityCount);
        }

        #endregion

        #region Reusing a Builder

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderReUseBuilder()
        {
            var builder = new EntityBuilder().With<TestComponent1>();

            var e1 = builder.Build(_scene);
            var e2 = builder.Build(_scene);

            Assert.AreNotEqual(e1, e2);
            Assert.AreEqual(1, e1.Components.Count);
            Assert.AreEqual(1, e2.Components.Count);
            Assert.AreEqual(2, _scene.EntityCount);
        }

        [TestCategory("Adding Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderReUseBuilderEditComponent()
        {
            var builder = new EntityBuilder().With<TestComponent1>();

            var e1 = builder.Build(_scene);
            var e2 = builder.Build(_scene);
            var newXVal = 100;

            var comp1 = e1.GetComponent<TestComponent1>();
            comp1.X = newXVal;

            var comp2 = e2.GetComponent<TestComponent1>();
            Assert.AreNotEqual(newXVal, comp2.X);
        }

        #endregion
    }
}