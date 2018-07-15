using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;
using ECSTests.TestHelper;
using ECS.Entities;
using ECS.Components.Exceptions;

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

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityTwoComponentCreationFunction()
        {
            var compX = 7;
            var e = new EntityBuilder()
                      .With(() => new TestComponent1() { X = compX, Y = 0 })
                      .With(() => new TestComponent2() { W = compX, Z = 0 })
                      .Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(2, e.Components.Count);
            var getComp = e.GetComponent<TestComponent1>();
            var getComp2 = e.GetComponent<TestComponent2>();
            Assert.AreEqual(compX, getComp.X);
            Assert.AreEqual(compX, getComp2.W);
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderEntityTwoSameComponentCreationFunction()
        {
            var compX = 7;
            var e = new EntityBuilder()
                      .With(() => new TestComponent1() { X = compX, Y = 0 })
                      .With(() => new TestComponent1() { X = compX + 10, Y = 0 })
                      .Build(_scene);
            Assert.AreEqual(1, _scene.EntityCount);
            Assert.AreEqual(1, e.Components.Count);
            var getComp = e.GetComponent<TestComponent1>();
            Assert.AreEqual(compX, getComp.X);
        }
        #endregion

        #region Removing Components

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderRemoveComponent()
        {
            var builder = new EntityBuilder().With<TestComponent1>();
            var e1 = builder.Build(_scene);
            Assert.AreEqual(1, e1.Components.Count);
            var e2 = builder.Without<TestComponent1>().Build(_scene);
            Assert.AreEqual(0, e2.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderRemoveOneComponent()
        {
            var builder = new EntityBuilder().With<TestComponent1>().With<TestComponent2>();
            var e1 = builder.Build(_scene);
            Assert.AreEqual(2, e1.Components.Count);
            var e2 = builder.Without<TestComponent1>().Build(_scene);
            Assert.AreEqual(1, e2.Components.Count);
            var e3 = builder.Without<TestComponent2>().Build(_scene);
            Assert.AreEqual(0, e3.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        [ExpectedException(typeof(UnregisteredComponentException))]
        public void EntityBuilderRemoveInvalidComponent()
        {
            var builder = new EntityBuilder().With<TestComponent1>();
            builder.Without<UnregisteredComponent>();
        }

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderRemoveComponentCreationFunction()
        {
            var builder = new EntityBuilder().With(() => new TestComponent1() { X = 10 });
            var e1 = builder.Build(_scene);
            Assert.AreEqual(1, e1.Components.Count);
            builder.Without<TestComponent1>();
            var e2 = builder.Build(_scene);
            Assert.AreEqual(0, e2.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderRemoveTwoComponentCreationFunctions()
        {
            var builder = new EntityBuilder()
                                .With(() => new TestComponent1() { X = 10 })
                                .With(() => new TestComponent2() { W = 10 });
            var e1 = builder.Build(_scene);
            Assert.AreEqual(2, e1.Components.Count);
            builder.Without<TestComponent1>();
            var e2 = builder.Build(_scene);
            Assert.AreEqual(1, e2.Components.Count);
            builder.Without<TestComponent2>();
            var e3 = builder.Build(_scene);
            Assert.AreEqual(0, e3.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderRemoveOneComponentCreationFunctionsAndRegular()
        {
            var builder = new EntityBuilder()
                                .With(() => new TestComponent1() { X = 10 })
                                .With<TestComponent2>();
            var e1 = builder.Build(_scene);
            Assert.AreEqual(2, e1.Components.Count);
            builder.Without<TestComponent1>();
            var e2 = builder.Build(_scene);
            Assert.AreEqual(1, e2.Components.Count);
            builder.Without<TestComponent2>();
            var e3 = builder.Build(_scene);
            Assert.AreEqual(0, e3.Components.Count);
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

        [TestCategory("EntityBuilder"), TestMethod]
        public void EntityBuilderCopy()
        {
            var builder1 = new EntityBuilder().With<TestComponent1>();
            var builder2 = builder1.Copy();

            Assert.AreNotEqual(builder1, builder2);

            builder2.Without<TestComponent1>();

            var e1 = builder1.Build(_scene);
            var e2 = builder2.Build(_scene);

            Assert.AreNotEqual(e1.Components.Count, e2.Components.Count);
        }

        #endregion
    }
}