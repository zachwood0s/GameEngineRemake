using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECS;
using ECSTests.TestHelper;
using ECS.Components.Exceptions;

namespace ECSTests.Entities
{
    [TestClass]
    public class BasicEntityTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Initialize()
        {
            ComponentHelper.RegisterTestComponents();
            _scene = new Scene();
        }

        #region Adding Components

        [TestCategory("Adding Components"), TestCategory("Generic"), TestMethod]
        public void AddComponentGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            entity.With<TestComponent1>();
            Assert.AreEqual(1, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestMethod]
        public void AddTwoComponentsGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            entity.With<TestComponent1>();
            entity.With<TestComponent2>();
            Assert.AreEqual(2, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestMethod]
        public void AddTwoComponentsOfSameTypeGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            entity.With<TestComponent1>();
            entity.With<TestComponent1>();
            Assert.AreEqual(1, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Generic"), TestMethod]
        [ExpectedException(typeof(UnregisteredComponentException))]
        public void AddUnregisteredComponentGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            entity.With<UnregisteredComponent>();
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestMethod]
        public void AddComponentNonGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            var comp = new TestComponent1();
            entity.With(comp);
            Assert.AreEqual(1, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestMethod]
        public void AddTwoComponentsNonGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            var comp1 = new TestComponent1();
            var comp2 = new TestComponent2();
            entity.With(comp1);
            entity.With(comp2);
            Assert.AreEqual(2, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestMethod]
        public void AddTwoComponentsOfSameTypeNonGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            var comp1 = new TestComponent1();
            var comp2 = new TestComponent1();
            entity.With(comp1);
            entity.With(comp2);
            Assert.AreEqual(1, entity.Components.Count);
        }

        [TestCategory("Adding Components"), TestCategory("Non-Generic"), TestMethod]
        [ExpectedException(typeof(UnregisteredComponentException))]
        public void AddUnregisteredComponentNonGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            var comp1 = new UnregisteredComponent();
            entity.With(comp1);
        }

        #endregion

        #region Removing Components

        [TestCategory("Removing Components"), TestCategory("Generic"), TestMethod]
        public void RemoveComponentGeneric()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            entity.Remove<TestComponent1>();
            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("Generic"), TestMethod]
        public void RemoveTwoComponentsGeneric()
        {
            var entity = EntityFactory.EntityWithTwoComponents(_scene);
            entity.Remove<TestComponent1>();
            entity.Remove<TestComponent2>();
            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("Generic"), TestMethod]
        [ExpectedException(typeof(UnregisteredComponentException))]
        public void RemoveUnregisteredComponentGeneric()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            entity.Remove<TestComponent1>();
        }

        [TestCategory("Removing Components"), TestCategory("Non-Generic"), TestMethod]
        public void RemoveComponentByType()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            entity.Remove(typeof(TestComponent1));
            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("Non-Generic"), TestMethod]
        public void RemoveComponentNonGeneric()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            var comp = new TestComponent1();
            entity.Remove(comp);
            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestCategory("Removing Components"), TestCategory("Non-Generic"), TestMethod]
        public void RemoveTwoComponentsNonGeneric()
        {
            var entity = EntityFactory.EntityWithTwoComponents(_scene);
            var comp1 = new TestComponent1();
            var comp2 = new TestComponent2();
            entity.Remove(comp1);
            entity.Remove(comp2);
            Assert.AreEqual(0, entity.Components.Count);
        }
        #endregion

        #region Getting Components

        [TestCategory("Getting Components"), TestMethod]
        public void GetComponentWithDefaults()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            var comp = entity.GetComponent<TestComponent1>();
            Assert.IsNotNull(comp);
            Assert.AreEqual(0, comp.X);
            Assert.AreEqual(0, comp.Y);
        }

        [TestCategory("Getting Components"), TestMethod]
        public void GetComponentWithoutDefaults()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            var comp = new TestComponent1
            {
                X = 100,
                Y = 50
            };
            entity.With(comp);

            var getComp = entity.GetComponent<TestComponent1>();
            Assert.IsNotNull(comp);
            Assert.AreEqual(100, comp.X);
            Assert.AreEqual(50, comp.Y);
        }

        [TestCategory("Getting Components"), TestMethod]
        public void GetInvalidComponent()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            Assert.IsNull(entity.GetComponent<TestComponent1>());
        }

        [TestCategory("Getting Components"), TestMethod]
        public void GetInvalidUnregisteredComponent()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            Assert.IsNull(entity.GetComponent<UnregisteredComponent>());
        }

        #endregion

        #region Checking For Components
        
        [TestCategory("Checking For Components"), TestMethod]
        public void HasComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            Assert.IsTrue(entity.HasComponent<TestComponent1>());
        }

        [TestCategory("Checking For Components"), TestMethod]
        public void HasInvalidComponent()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            Assert.IsFalse(entity.HasComponent<TestComponent1>());
        }

        [TestCategory("Checking For Components"), TestMethod]
        public void HasInvalidUnregisteredComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            Assert.IsFalse(entity.HasComponent<UnregisteredComponent>());
        }

        #endregion

        #region Updating Components

        [TestCategory("Updating Components"), TestMethod]
        public void UpdateComponentWithNewComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            int newX = 100;
            int newY = 200;

            var newComp = new TestComponent1()
            {
                X = newX,
                Y = newY
            };

            entity.UpdateComponent(newComp);

            var updatedComp = entity.GetComponent<TestComponent1>();
            Assert.IsNotNull(updatedComp);
            Assert.AreEqual(newX, updatedComp.X);
            Assert.AreEqual(newY, updatedComp.Y);
        }

        [TestCategory("Updating Components"), TestMethod]
        public void UpdateComponentWithExistingComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            var comp = entity.GetComponent<TestComponent1>();
            int newX = 100;
            int newY = 200;

            comp.X = newX;
            comp.Y = newY;

            entity.UpdateComponent(comp);

            var updatedComp = entity.GetComponent<TestComponent1>();
            Assert.IsNotNull(updatedComp);
            Assert.AreEqual(newX, updatedComp.X);
            Assert.AreEqual(newY, updatedComp.Y);
        }

        [TestCategory("Updating Components"), TestMethod]
        [ExpectedException(typeof(UnregisteredComponentException))]
        public void UpdateInvalidComponent()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            int newX = 100;
            int newY = 200;

            var comp = new TestComponent2()
            {
                W = newX,
                Z = newY
            };

            entity.UpdateComponent(comp);
        }

        #endregion

        #region Subscribing/Unsubscribing

        [TestCategory("Adding Components"), TestCategory("Subscribing"), TestMethod]
        public void AddEventHandlerCalledOnAdd()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            bool addHandlerWasCalled = false;

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    addHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler,
                handler);

            entity.With<TestComponent1>();

            Assert.IsTrue(addHandlerWasCalled);
        }
        
        [TestCategory("Removing Components"), TestCategory("Subscribing"), TestMethod]
        public void RemoveEventHandlerCalledOnRemove()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            bool removeHandlerWasCalled = false;

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    removeHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                handler,
                EntityFactory.EmptyEntityChangedHandler
                );

            entity.Remove<TestComponent1>();

            Assert.IsTrue(removeHandlerWasCalled);
        }

        [TestCategory("Updating Components"), TestCategory("Subscribing"), TestMethod]
        public void UpdateEventHandlerCalledOnUpdate()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            int newX = 100;
            int newY = 200;
            bool updateHandlerWasCalled = false;

            var newComp = new TestComponent1()
            {
                X = newX,
                Y = newY
            };

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    updateHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                handler, 
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler);

            entity.UpdateComponent(newComp);

            Assert.IsTrue(updateHandlerWasCalled);
        }

        [TestCategory("Adding Components"), TestCategory("Unsubscribing"), TestMethod]
        public void AddEventHandlerWasNotCalledOnAddAfterUnsubscribe()
        {
            var entity = EntityFactory.EntityWithNoComponents(_scene);
            bool addHandlerWasCalled = false;

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    addHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler,
                handler);
            entity.UnSubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler,
                handler);
            entity.With<TestComponent1>();

            Assert.IsFalse(addHandlerWasCalled);
        }
        
        [TestCategory("Removing Components"), TestCategory("Unsubscribing"), TestMethod]
        public void RemoveEventHandlerWasNotCalledOnRemoveAfterUnsubscribe()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            bool removeHandlerWasCalled = false;

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    removeHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                handler,
                EntityFactory.EmptyEntityChangedHandler
                );
            entity.UnSubscribeToChanges(
                EntityFactory.EmptyEntityChangedHandler, 
                handler,
                EntityFactory.EmptyEntityChangedHandler
                );

            entity.Remove<TestComponent1>();

            Assert.IsFalse(removeHandlerWasCalled);
        }

        [TestCategory("Updating Components"), TestCategory("Unsubscribing"), TestMethod]
        public void UpdateEventHandlerWasNotCalledOnUpdateAfterUnsubscribe()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            int newX = 100;
            int newY = 200;
            bool updateHandlerWasCalled = false;

            var newComp = new TestComponent1()
            {
                X = newX,
                Y = newY
            };

            EntityChangedEventHandler handler =
                (upEntity, index, component) =>
                {
                    updateHandlerWasCalled = true;
                };

            entity.SubscribeToChanges(
                handler, 
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler);
            entity.UnSubscribeToChanges(
                handler, 
                EntityFactory.EmptyEntityChangedHandler, 
                EntityFactory.EmptyEntityChangedHandler);

            entity.UpdateComponent(newComp);

            Assert.IsFalse(updateHandlerWasCalled);
        }

        #endregion

        #region Update Function

        [TestCategory("Entity Update Function"), TestCategory("Updating Components"), TestMethod]
        public void UpdateComponentWithUpdateFunction()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            entity.UpdateComponent<TestComponent1>(
                c => c.X = 200
                );

            var comp = entity.GetComponent<TestComponent1>();
            Assert.AreEqual(200, comp.X);
        }

        [TestCategory("Entity Update Function"), TestCategory("Updating Components"), TestMethod]
        public void UpdateInvalidComponentWithUpdateFunction()
        {
            var entity = EntityFactory.EntityWithOneComponent(_scene);
            bool neverRuns = true;
            entity.UpdateComponent<TestComponent2>(c =>
            {
                c.W = 200;
                neverRuns = false;
            });

            Assert.IsTrue(neverRuns);
            var comp = entity.GetComponent<TestComponent1>();
            Assert.AreEqual(0, comp.X);
        }

        [TestCategory("Entity Update Function"), TestCategory("Updating Components"), TestMethod]
        public void UpdateComponentWithUpdateFunctionNested()
        {
            var entity = EntityFactory.EntityWithTwoComponents(_scene);
            int newX = 200;
            int newW = 300;
            entity.UpdateComponent<TestComponent1>(c =>
            {
                c.X = newX;

                entity.UpdateComponent<TestComponent2>(c2 =>
                {
                    c2.W = newW;
                });
            });

            var comp1 = entity.GetComponent<TestComponent1>();
            var comp2 = entity.GetComponent<TestComponent2>();
            Assert.AreEqual(newX, comp1.X);
            Assert.AreEqual(newW, comp2.W);
        }

        [TestCategory("Entity Update Function"), TestCategory("Updating Components"), TestMethod]
        public void UpdateComponentsWithUpdateFunction()
        {
            var entity = EntityFactory.EntityWithTwoComponents(_scene);
            int newX = 200;
            int newW = 300;
            entity.UpdateComponents<TestComponent1, TestComponent2>((c1, c2) =>
            {
                c1.X = newX;
                c2.W = newW;
            });

            var comp1 = entity.GetComponent<TestComponent1>();
            var comp2 = entity.GetComponent<TestComponent2>();
            Assert.AreEqual(newX, comp1.X);
            Assert.AreEqual(newW, comp2.W);
        }
        
        #endregion
    }
}
