using ECS;
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
    public class GroupTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            ComponentPool.RegisterComponent<TestPositionComponent>();
            ComponentPool.RegisterComponent<TestPositionComponent2>();

            _scene = new Scene();
        }

        [TestMethod]
        public void SimpleGetGroup()
        {
            
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());

            Assert.IsNotNull(positions);
            Assert.AreEqual(1, positions.EntityCount);
        }

        [TestMethod]
        public void GroupWithTwoAllOf()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>().With<TestPositionComponent2>();
            Entity test2 = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());
            Group positions2 = _scene.GetGroup(new Matcher().AllOf(typeof(TestPositionComponent), typeof(TestPositionComponent2)));

            Assert.IsNotNull(positions1);
            Assert.AreEqual(2, positions1.EntityCount);
            Assert.AreEqual(1, positions2.EntityCount);
            
        }

        [TestMethod]
        public void GroupWithNoneOf()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>().With<TestPositionComponent2>();
            Entity test2 = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().NoneOf(typeof(TestPositionComponent2)));

            Assert.IsNotNull(positions1);
            Assert.AreEqual(test2, positions1[0]);
            Assert.AreEqual(1, positions1.EntityCount);
        }

        [TestMethod]
        public void GroupWithAnyOf()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent2>();
            Entity test2 = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().AnyOf(typeof(TestPositionComponent),typeof(TestPositionComponent2)));

            Assert.IsNotNull(positions1);
            Assert.AreEqual(2, positions1.EntityCount);
        }

        [TestMethod]
        public void EmptyGroup()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent2>();
            Group positions1 = _scene.GetGroup(new Matcher().AnyOf(typeof(TestPositionComponent)));

            Assert.IsNotNull(positions1);
            Assert.AreEqual(0, positions1.EntityCount);
        }

        [TestMethod]
        public void GroupUpdateWithEntityAdd()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());

            Entity test2 = _scene.CreateEntity().With<TestPositionComponent>().With<TestPositionComponent2>();
            Entity test3 = _scene.CreateEntity().With<TestPositionComponent>();

            Assert.IsNotNull(positions1);
            Assert.AreEqual(3, positions1.EntityCount);
        }

        [TestMethod]
        public void GroupEntityNoLongerValid()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().Of<TestPositionComponent>().NoneOf(typeof(TestPositionComponent2)));

            Assert.AreEqual(1, positions1.EntityCount);

            test.With<TestPositionComponent2>();
            Assert.AreEqual(0, positions1.EntityCount);
        }

        [TestMethod]
        public void GroupEntityComponentRemoved()
        {
            Entity test = _scene.CreateEntity().With<TestPositionComponent>();
            Group positions1 = _scene.GetGroup(new Matcher().Of<TestPositionComponent>());

            Assert.AreEqual(1, positions1.EntityCount);

            test.Remove<TestPositionComponent>();
            Assert.AreEqual(0, positions1.EntityCount);
        }


        [TestMethod]
        public void GroupWithFilter()
        {
            TestPositionComponent comp = new TestPositionComponent
            {
                X = 10,
                Y = 20
            };
            Entity test = _scene.CreateEntity().With(comp);
            Group positions = _scene.GetGroup(
                new Matcher()
                    .Of<TestPositionComponent>()
                    .WithFilter(e =>
                    {
                        TestPositionComponent c = e.GetComponent<TestPositionComponent>();
                        return c?.X > 0;
                    }));

            Assert.AreEqual(1, positions.EntityCount);

            comp.X = -20;
            test.UpdateComponent(comp);

            Assert.AreEqual(0, positions.EntityCount);
        }

    }
}
