using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;
using ECSTests.TestHelper;
using ECS.Matching;

namespace ECSTests.Groups
{
    [TestClass]
    public class BasicGroupTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Initialize()
        {
            ComponentHelper.RegisterTestComponents();
            _scene = new Scene();
        }

        #region All Of

        [TestCategory("Groups All Of"), TestMethod]
        public void GroupAllOfOneComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            var group = _scene.GetGroup(new Matcher().Of<TestComponent1>());

            Assert.IsNotNull(group);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestCategory("Groups All Of"), TestMethod]
        public void GroupAllOfTwoComponent()
        {
            var entity = EntityFactory.EntityWithTwoComponents(_scene);
            var group = _scene.GetGroup(
                new Matcher().AllOf(typeof(TestComponent1), typeof(TestComponent2))
                );

            Assert.IsNotNull(group);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestCategory("Groups All Of"), TestMethod]
        public void GroupAllOfTwoComponentEmpty()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            var group = _scene.GetGroup(
                new Matcher().AllOf(typeof(TestComponent1), typeof(TestComponent2))
                );

            Assert.IsNotNull(group);
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        #endregion

        #region None Of

        [TestCategory("Groups None Of"), TestMethod]
        public void GroupNoneOf()
        {
            var entity1 = EntityFactory.EntityWithOneComponent(_scene);
            var entity2 = EntityFactory.EntityWithTwoComponents(_scene);
            var group = _scene.GetGroup(
                new Matcher().NoneOf(typeof(TestComponent2))
                );

            Assert.IsNotNull(group);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestCategory("Groups None Of"), TestMethod]
        public void GroupBothEnitiesFailNoneOf()
        {
            var entity1 = EntityFactory.EntityWithOneComponent(_scene);
            var entity2 = EntityFactory.EntityWithTwoComponents(_scene);
            var group = _scene.GetGroup(
                new Matcher().NoneOf(typeof(TestComponent1))
                );

            Assert.IsNotNull(group);
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        #endregion

        #region Any Of

        [TestCategory("Groups Any Of"), TestMethod]
        public void GroupAnyOf()
        {
            var entity1 = EntityFactory.EntityWithOneComponent(_scene);
            var entity2 = _scene.CreateEntity().With<TestComponent2>();
            var group = _scene.GetGroup(
                new Matcher().AnyOf(typeof(TestComponent2), typeof(TestComponent1))
                );

            Assert.IsNotNull(group);
            Assert.AreEqual(2, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        #endregion

        [TestMethod]
        public void GroupAddEntityAfterGroupCreation()
        {
            var test1 = EntityFactory.EntityWithTwoComponents(_scene);
            var group = _scene.GetGroup(new Matcher().Of<TestComponent1>());

            var test2 = EntityFactory.EntityWithOneComponent(_scene);
            var test3 = EntityFactory.EntityWithTwoComponents(_scene);

            Assert.IsNotNull(group);
            Assert.AreEqual(3, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupEntityComponentRemoved()
        {
            var test = EntityFactory.EntityWithOneComponent(_scene);
            var group = _scene.GetGroup(new Matcher().Of<TestComponent1>());

            test.Remove<TestComponent1>();
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupEntityNoLongerValid()
        {
            var test = EntityFactory.EntityWithOneComponent(_scene);
            var group = _scene.GetGroup(new Matcher().Of<TestComponent1>().NoneOf(typeof(TestComponent2)));

            test.With<TestComponent2>();
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupWithFilter()
        {
            TestComponent1 comp = new TestComponent1
            {
                X = 10,
                Y = 20
            };
            Entity test = _scene.CreateEntity().With(comp);
            Group group = _scene.GetGroup(
                new Matcher()
                    .Of<TestComponent1>()
                    .WithFilter(e =>
                    {
                        var c = e.GetComponent<TestComponent1>();
                        return c?.X > 0;
                    }));

            Assert.AreEqual(1, group.EntityCount);

            comp.X = -20;
            test.UpdateComponent(comp);

            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(1, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupWithFilterEntityRemovedAndThenAddedBack()
        {
            var comp = new TestComponent1
            {
                X = 10,
                Y = 20
            };
            var test = _scene.CreateEntity().With(comp);
            var group = _scene.GetGroup(
                new Matcher()
                .Of<TestComponent1>()
                .WithFilter(e =>
                {
                    var c = e.GetComponent<TestComponent1>();
                    return c?.X > 0;
                }));

            comp.X = -20;
            //Should remove the entity from the group
            test.UpdateComponent(comp);
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(1, group.CachedEntityCount);

            comp.X = 20;
            //Should add the entity back into the group
            test.UpdateComponent(comp);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupWithFilterEntityAddedAfterGroupCreation()
        {
            var comp = new TestComponent1
            {
                X = 10,
                Y = 20
            };

            var group = _scene.GetGroup(
                new Matcher()
                .Of<TestComponent1>()
                .WithFilter(e =>
                {
                    var c = e.GetComponent<TestComponent1>();
                    return c?.X > 0;
                }));

            var test = _scene.CreateEntity().With(comp);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

        [TestMethod]
        public void GroupWithFilterEntityAddedAfterGroupCreationWasInvalidAtFirst()
        {
            var comp = new TestComponent1
            {
                X = -10,
                Y = 20
            };

            var group = _scene.GetGroup(
                new Matcher()
                .Of<TestComponent1>()
                .WithFilter(e =>
                {
                    var c = e.GetComponent<TestComponent1>();
                    return c?.X > 0;
                }));

            var test = _scene.CreateEntity().With(comp);
            Assert.AreEqual(0, group.EntityCount);
            Assert.AreEqual(1, group.CachedEntityCount);
            comp.X = 10;
            test.UpdateComponent(comp);
            Assert.AreEqual(1, group.EntityCount);
            Assert.AreEqual(0, group.CachedEntityCount);
        }

    }
}
