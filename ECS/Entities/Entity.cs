using ECS.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ECS.Components.Exceptions;
using ECS.Matching;

namespace ECS.Entities
{
    public delegate void EntityChangedEventHandler(Entity entity, int componentPoolIndex, IComponent component);

    /// <summary>
    /// The Entity class holds components that define the properties of an entity
    /// It has methods for adding/removing/updating components. Only one component
    /// of each type can be stored in an entity. An entity cannot be created directly.
    /// It needs to be created buy an <see cref="EntityBuilder"/> or a <see cref="Scene"/>
    /// All the private methods in this class assume that the proper locks have already been
    /// taken and therefore do no locking of their own.
    /// </summary>
    public class Entity
    {
        private ReaderWriterLockSlim _readerWriterLock;
        private List<IComponent> _components;
        private List<int> _componentTypeIndicies;
        private HashSet<int> _componentTypeIndiciesLookup;

        private event EntityChangedEventHandler _OnComponentUpdated;
        private event EntityChangedEventHandler _OnComponentRemoved;
        private event EntityChangedEventHandler _OnComponentAdded;

        public EntityChangedEventHandler OnComponentUpdated => _OnComponentUpdated;
        internal Entity()
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _components = new List<IComponent>();
            _componentTypeIndicies = new List<int>();
            _componentTypeIndiciesLookup = new HashSet<int>();
        }

        internal Entity(List<IComponent> components, List<int> componentTypeIndicies)
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _components = new List<IComponent>(components);
            _componentTypeIndicies = new List<int>(componentTypeIndicies);
            _componentTypeIndiciesLookup = new HashSet<int>(componentTypeIndicies);
        }

        #region Getters/Setters

        /// <summary>
        /// Returns a list of components that an entity has.
        /// It's a readonly list and the components in the
        /// list should not be changed. Should probably only
        /// be used for debugging purposes. If you need to query
        /// components you should use GetComponent or HasComponent.
        /// NOT THREAD SAFE!
        /// </summary>
        /// <seealso cref="GetComponent{T}"/>
        /// <seealso cref="HasComponent{T}"/>
        public IReadOnlyList<IComponent> Components => _components;
        /// <summary>
        /// Returns a list of components type indices that an entity has.
        /// It's a readonly list and should probably only
        /// be used for debugging purposes. If you need to query
        /// components you should use GetComponent or HasComponent
        /// NOT THREAD SAFE!
        /// <seealso cref="GetComponent{T}"/>
        /// <seealso cref="HasComponent{T}"/>
        /// </summary>
        public IReadOnlyList<int> ComponentTypeIndicies => _componentTypeIndicies;

        #endregion

        #region Subscribing/Unsubscribing

        /// <summary>
        /// Subscribes this entity to event handlers.
        /// </summary>
        /// <param name="updated">The handler for when a component has been updated</param>
        /// <param name="removed">The handler for when a component has been removed</param>
        /// <param name="added">The handler for when a component has been added</param>
        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _readerWriterLock.EnterWriteLock();
            _OnComponentUpdated += updated;
            _OnComponentRemoved += removed;
            _OnComponentAdded += added;
            _readerWriterLock.ExitWriteLock();
        }

        /// <summary>
        /// Unsubscribes this entity to event handlers.
        /// </summary>
        /// <param name="updated">The handler for when a component has been updated</param>
        /// <param name="removed">The handler for when a component has been removed</param>
        /// <param name="added">The handler for when a component has been added</param>
        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _readerWriterLock.EnterWriteLock();
            _OnComponentUpdated -= updated;
            _OnComponentRemoved -= removed;
            _OnComponentAdded -= added;
            _readerWriterLock.ExitWriteLock();
        }

        #endregion

        #region Component Checking

        /// <summary>
        /// Retrieves the component of type T if it exists, returns null otherwise
        /// </summary>
        /// <typeparam name="T">Component type that must be an IComponent</typeparam>
        /// <returns>The component of type T or null if it doesn't exist</returns>
        public T GetComponent<T>() where T : class, IComponent
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                return _GetComponent<T>();
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        private T _GetComponent<T>() where T: class, IComponent
        {
            foreach (IComponent comp in _components)
            {
                if (comp is T)
                {
                    return (T)comp;
                }
            }
            return null;
        }

        #region GetComponent Action Overloads

        /// <summary>
        /// Retrieves the component of type T and applies an action to that component.
        /// If the component doesn't exist, the action is not applied. 
        /// 
        /// NOTE: The action must NOT change the component or call the entities UpdateComponent
        /// method from within the action. It is intended to be readonly. If you change the 
        /// state of a component, the proper groups will not be notified and will have unintended
        /// consequences. If you call the UpdateComponent method from within an exception will be thrown!
        /// This is because you would be attempting to enter a write lock from within a read lock.
        /// </summary>
        /// <example>
        /// An example call of <see cref="GetComponent{T}(Action{T})"/>
        /// <code>
        /// entity.GetComponent<ComponentType>(component => Console.WriteLine(component.ex));
        /// </code>
        /// </example>
        /// <typeparam name="T">Component type that must be an IComponent</typeparam>
        /// <param name="getAction">The action to apply to the retrieved component</param>
        public void GetComponent<T>(Action<T> getAction) where T: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                T comp = _GetComponent<T>();
                if (comp != null)
                {
                    getAction(comp);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Retrieves the component of each type T* and applies an action to those components.
        /// If any one of the components doesn't exist, the action is not applied. 
        /// 
        /// NOTE: The action must NOT change the component or call the entities UpdateComponent
        /// method from within the action. It is intended to be readonly. If you change the 
        /// state of a component, the proper groups will not be notified and will have unintended
        /// consequences. If you call the UpdateComponent method from within an exception will be thrown!
        /// This is because you would be attempting to enter a write lock from within a read lock.
        /// </summary>
        /// <example>
        /// <code>
        /// entity.GetComponents<ComponentType1, ComponentType2>(
        ///     (comp1, comp2) => Console.WriteLine(comp1.ex)
        ///  );
        /// </code>
        /// </example>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <param name="getAction">The action to apply to the retrieved components</param>
        public void GetComponents<T1, T2>(Action<T1, T2> getAction) where T1: class, IComponent
                                                                   where T2: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                T1 comp1 = _GetComponent<T1>(); T2 comp2 = _GetComponent<T2>();
                if (comp1 != null && comp2 != null)
                {
                    getAction(comp1, comp2);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Retrieves the component of each type T* and applies an action to those components.
        /// If any one of the components doesn't exist, the action is not applied. 
        /// 
        /// NOTE: The action must NOT change the component or call the entities UpdateComponent
        /// method from within the action. It is intended to be readonly. If you change the 
        /// state of a component, the proper groups will not be notified and will have unintended
        /// consequences. If you call the UpdateComponent method from within an exception will be thrown!
        /// This is because you would be attempting to enter a write lock from within a read lock.
        /// </summary>
        /// <example>
        /// <code>
        /// entity.GetComponents<ComponentType1, ComponentType2, ComponentType3>(
        ///     (comp1, comp2, comp3) => Console.WriteLine(comp1.ex)
        /// );
        /// </code>
        /// </example>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <param name="getAction">The action to apply to the retrieved components</param>
        public void GetComponents<T1, T2, T3>(Action<T1, T2, T3> getAction) where T1: class, IComponent
                                                                           where T2: class, IComponent
                                                                           where T3: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                T1 comp1 = _GetComponent<T1>(); T2 comp2 = _GetComponent<T2>(); T3 comp3 = _GetComponent<T3>();
                if (comp1 != null && comp2 != null && comp3 != null)
                {
                    getAction(comp1, comp2, comp3);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Retrieves the component of each type T* and applies an action to those components.
        /// If any one of the components doesn't exist, the action is not applied. 
        /// 
        /// NOTE: The action must NOT change the component or call the entities UpdateComponent
        /// method from within the action. It is intended to be readonly. If you change the 
        /// state of a component, the proper groups will not be notified and will have unintended
        /// consequences. If you call the UpdateComponent method from within an exception will be thrown!
        /// This is because you would be attempting to enter a write lock from within a read lock.
        /// </summary>
        /// <example>
        /// <code>
        /// entity.GetComponents<ComponentType1, ComponentType2, ComponentType3, ComponentType4>(
        ///     (comp1, comp2, comp3, comp4) => Console.WriteLine(comp1.ex)
        /// );
        /// </code>
        /// </example>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T4">Component type that must be an IComponent</typeparam>
        /// <param name="getAction">The action to apply to the retrieved components</param>
        public void GetComponents<T1, T2, T3, T4>(Action<T1, T2, T3, T4> getAction) where T1: class, IComponent
                                                                                   where T2: class, IComponent
                                                                                   where T3: class, IComponent
                                                                                   where T4: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                T1 comp1 = _GetComponent<T1>(); T2 comp2 = _GetComponent<T2>(); T3 comp3 = _GetComponent<T3>(); T4 comp4 = _GetComponent<T4>();
                if (comp1 != null && comp2 != null && comp3 != null && comp4 != null)
                {
                    getAction(comp1, comp2, comp3, comp4);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Retrieves the component of each type T* and applies an action to those components.
        /// If any one of the components doesn't exist, the action is not applied. 
        /// 
        /// NOTE: The action must NOT change the component or call the entities UpdateComponent
        /// method from within the action. It is intended to be readonly. If you change the 
        /// state of a component, the proper groups will not be notified and will have unintended
        /// consequences. If you call the UpdateComponent method from within an exception will be thrown!
        /// This is because you would be attempting to enter a write lock from within a read lock.
        /// </summary>
        /// <example>
        /// <code>
        /// entity.GetComponents<ComponentType1, ComponentType2, ComponentType3, ComponentType4, ComponentType5>(
        ///     (comp1, comp2, comp3, comp4, comp5) => Console.WriteLine(comp1.ex)
        /// );
        /// </code>
        /// </example>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T4">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T5">Component type that must be an IComponent</typeparam>
        /// <param name="getAction">The action to apply to the retrieved components</param>
        public void GetComponents<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> getAction) where T1: class, IComponent
                                                                                           where T2: class, IComponent
                                                                                           where T3: class, IComponent
                                                                                           where T4: class, IComponent
                                                                                           where T5: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                T1 comp1 = _GetComponent<T1>(); T2 comp2 = _GetComponent<T2>(); T3 comp3 = _GetComponent<T3>(); T4 comp4 = _GetComponent<T4>(); T5 comp5 = _GetComponent<T5>(); if (comp1 != null && comp2 != null && comp3 != null && comp4 != null && comp5 != null)
                {
                    getAction(comp1, comp2, comp3, comp4, comp5);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }
        #endregion


        /// <summary>
        /// Checks to see if an entity has a component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type that must be an IComponent</typeparam>
        /// <returns>True if the entity contains the component, false otherwise</returns>
        public bool HasComponent<T>() where T: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return _components.Exists(c => c is T);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Checks to see if an entity has a component with the specified component pool
        /// index. This index can be retrieved from the <see cref="ComponentPool.GetComponentIndex(Type)"/> method 
        /// but at that point you might as well just use <see cref="HasComponent{T}"/>
        /// </summary>
        /// <param name="componentPoolIndex">The index of the component in the component pool</param>
        /// <returns>True if the entity contains the component, false otherwise</returns>
        public bool HasComponent(int componentPoolIndex)
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return _componentTypeIndiciesLookup.Contains(componentPoolIndex);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        #endregion

        #region Adding Components
        /// <summary>
        /// Adds the given component to the entity if a component of the same type
        /// is not already in the entity.
        /// </summary>
        /// <param name="newComp">The new component to add</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public Entity With(IComponent newComp)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _With(newComp);
                return this;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Creates a component of type T and adds it to the entity if a compoenent of the 
        /// same type is not already in the entity.
        /// </summary>
        /// <typeparam name="T">Component type that must be an IComponentHasDefault</typeparam>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public Entity With<T>() where T : IComponentHasDefault
        {
            T newComp = Activator.CreateInstance<T>();
            newComp.SetDefaults();

            _readerWriterLock.EnterWriteLock();
            try
            {
                _With(newComp);
                return this;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private void _With(IComponent newComp)
        {
            int newCompIndex = ComponentPool.GetComponentIndex(newComp.GetType());

            if (newCompIndex < 0)
                throw new UnregisteredComponentException($"Component of type {newComp.GetType()} has not been registered with the ComponentPool.");

            if (!_componentTypeIndiciesLookup.Contains(newCompIndex))
            {
                _components.Add(newComp);
                _componentTypeIndicies.Add(newCompIndex);
                _componentTypeIndiciesLookup.Add(newCompIndex);

                //Notify the listeners that a component has been added
                _OnComponentAdded?.Invoke(this, newCompIndex, newComp);
            }
        }
        #endregion

        #region Removing Components

        /// <summary>
        /// Removes a component of the specified type
        /// </summary>
        /// <param name="oldCompType">The type of component to remove</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public Entity Remove(Type oldCompType)
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                int oldCompIndex = ComponentPool.GetComponentIndex(oldCompType);
                _Remove(oldCompIndex);
                return this;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes the component from the entity
        /// </summary>
        /// <param name="oldComp">The component to remove</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public Entity Remove(IComponent oldComp)
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                return Remove(oldComp.GetType());
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes the component of type T from the entity
        /// </summary>
        /// <typeparam name="T">Component type where component is IComponent</typeparam>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public Entity Remove<T>() where T : IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                int oldCompIndex = ComponentPool.GetComponentIndex<T>();
                _Remove(oldCompIndex);
                return this;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private void _Remove(int oldCompIndex)
        {
            int compIndex = _componentTypeIndicies.IndexOf(oldCompIndex);

            if(compIndex < 0)
                throw new UnregisteredComponentException($"Component of type " +
                    $"{ComponentPool.Components[oldCompIndex].GetType()} has not been added to the current entity and is trying to be removed.");

            IComponent oldComp = _components[compIndex];
            _components.RemoveAt(compIndex);
            _componentTypeIndicies.RemoveAt(compIndex);
            _componentTypeIndiciesLookup.Remove(oldCompIndex);

            //Notify the listeners that a component has been removed
            _OnComponentRemoved?.Invoke(this, oldCompIndex, oldComp);
        }

        #endregion

        #region Updating Components

        /// <summary>
        /// Replaces the component of type T that is currently stored in 
        /// the entity with the one that is passed in
        /// </summary>
        /// <typeparam name="T">Type of component to replace that implements IComponent</typeparam>
        /// <param name="component">The component to replace with the existing component</param>
        public void UpdateComponent<T>(T component) where T:class, IComponent
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _UpdateComponent(component);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
           
        }

        private void _UpdateComponent<T>(T component) where T: class, IComponent
        {
            int newCompPoolIndex = ComponentPool.GetComponentIndex<T>();
            int existingCompIndex = _componentTypeIndicies.IndexOf(newCompPoolIndex);

            if (existingCompIndex < 0)
                return;

            _components[existingCompIndex] = component;

            //Notify the listeners that a component has been updated
            _OnComponentUpdated?.Invoke(this, newCompPoolIndex, component);
        }

        #region UpdateComponent Action Overloads
        /*
         * TODO:
         * I think these functions could be sped up every so slightly by not
         * actually calling the UpdateComponent() base method because it does
         * other things that these functions have already guaranteed. (ex: 
         * it checks whether the component is already in the entity, which in
         * these cases we already know it is.) We should be able to just notify
         * all of our watchers
         */
        /// <summary>
        /// Updates the component of type T and applies an action to that component.
        /// If the component doesn't exist, the action is not applied. 
        /// 
        /// NOTE: This is the preferred method over <see cref="GetComponent{T}(Action{T})"/> only
        /// if the action MUST change the state of the component. If not, use <see cref="GetComponent{T}(Action{T})"/>
        /// for improved performance.
        /// </summary>
        /// <example>
        /// An example call of <see cref="UpdateComponent{T}(Action{T})"/>
        /// <code>
        /// entity.UpdateComponent<ComponentType>(component => component.ex = 2);
        /// </code>
        /// </example>
        /// <typeparam name="T">Component type that must be an IComponent</typeparam>
        /// <param name="updateAction">The action to apply to the retrieved component</param>
        public void UpdateComponent<T>(Action<T> updateAction) where T: class, IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                T comp = _GetComponent<T>();
                if (comp != null)
                {
                    updateAction(comp);
                    _UpdateComponent(comp);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Updates the components of type T* and applies an action to those components.
        /// If any of the components don't exist, the action is not applied. 
        /// 
        /// NOTE: This is the preferred method over <see cref="GetComponents{T1, T2}(Action{T1, T2})"/> only
        /// if the action MUST change the state of the component. If not, use <see cref="GetComponents{T1, T2}(Action{T1, T2})"/>
        /// for improved performance.
        /// </summary>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <param name="updateAction">The action to apply to the retrieved component</param>
        public void UpdateComponents<T1, T2>(Action<T1, T2> updateAction) where T1: class, IComponent 
                                                                          where T2: class, IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                T1 c1 = _GetComponent<T1>(); T2 c2 = _GetComponent<T2>();
                if (c1 != null && c2 != null)
                {
                    updateAction(c1, c2);
                    _UpdateComponent(c1); _UpdateComponent(c2);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Updates the components of type T* and applies an action to those components.
        /// If any of the components don't exist, the action is not applied. 
        /// 
        /// NOTE: This is the preferred method over <see cref="GetComponents{T1, T2, T3}(Action{T1, T2, T3})"/> only
        /// if the action MUST change the state of the component. If not, use <see cref="GetComponents{T1, T2, T3}(Action{T1, T2, T3})"/>
        /// for improved performance.
        /// </summary>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <param name="updateAction">The action to apply to the retrieved component</param>
        public void UpdateComponents<T1, T2, T3>(Action<T1, T2, T3> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                T1 c1 = _GetComponent<T1>(); T2 c2 = _GetComponent<T2>(); T3 c3 = _GetComponent<T3>();
                if (c1 != null && c2 != null && c3 != null)
                {
                    updateAction(c1, c2, c3);
                    _UpdateComponent(c1); _UpdateComponent(c2); _UpdateComponent(c3);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Updates the components of type T* and applies an action to those components.
        /// If any of the components don't exist, the action is not applied. 
        /// 
        /// NOTE: This is the preferred method over <see cref="GetComponents{T1, T2, T3, T4}(Action{T1, T2, T3, T4})"/> only
        /// if the action MUST change the state of the component. If not, use <see cref="GetComponents{T1, T2, T3, T4}(Action{T1, T2, T3, T4})"/>
        /// for improved performance.
        /// </summary>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T4">Component type that must be an IComponent</typeparam>
        /// <param name="updateAction">The action to apply to the retrieved component</param>
        public void UpdateComponents<T1, T2, T3, T4>(Action<T1, T2, T3, T4> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
                                                                                  where T4: class, IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                T1 c1 = _GetComponent<T1>(); T2 c2 = _GetComponent<T2>(); T3 c3 = _GetComponent<T3>(); T4 c4 = _GetComponent<T4>();
                if (c1 != null && c2 != null && c3 != null && c4 != null)
                {
                    updateAction(c1, c2, c3, c4);
                    _UpdateComponent(c1); _UpdateComponent(c2); _UpdateComponent(c3); _UpdateComponent(c4);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Updates the components of type T* and applies an action to those components.
        /// If any of the components don't exist, the action is not applied. 
        /// 
        /// NOTE: This is the preferred method over <see cref="GetComponents{T1, T2, T3, T4, T5}(Action{T1, T2, T3, T4, T5})"/> only
        /// if the action MUST change the state of the component. If not, use <see cref="GetComponents{T1, T2, T3, T4, T5}(Action{T1, T2, T3, T4, T5})"/>
        /// for improved performance.
        /// </summary>
        /// <typeparam name="T1">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T2">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T3">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T4">Component type that must be an IComponent</typeparam>
        /// <typeparam name="T5">Component type that must be an IComponent</typeparam>
        /// <param name="updateAction">The action to apply to the retrieved component</param>
        public void UpdateComponents<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
                                                                                  where T4: class, IComponent
                                                                                  where T5: class, IComponent
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                T1 c1 = _GetComponent<T1>(); T2 c2 = _GetComponent<T2>(); T3 c3 = _GetComponent<T3>(); T4 c4 = _GetComponent<T4>(); T5 c5 = _GetComponent<T5>();
                if (c1 != null && c2 != null && c3 != null && c4 != null && c5 != null)
                {
                    updateAction(c1, c2, c3, c4, c5);
                    _UpdateComponent(c1); _UpdateComponent(c2); _UpdateComponent(c3); _UpdateComponent(c4); _UpdateComponent(c5);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }
        #endregion

        #endregion

        #region Matching

        /// <summary>
        /// Checks to see if this entity is a match with a Matcher without
        /// using the filter part of a Matcher. Meaning it'll check to see if it
        /// has AllOf/NoneOf/AnyOf a component but will not check to see if those components
        /// meet a certain condition. We needed to split this function out because without
        /// it a <see cref="Group"/> would remove the entity when the filter was failed, but
        /// it wouldn't be notified when the filter succeeded again. So now it checks to see if
        /// it's only failing because of the filter, and if so it adds it in a cached entites
        /// list but removes it from the observed entities
        /// </summary>
        /// <param name="match">The <see cref="Matcher"/> to test the entity against</param>
        /// <returns>True if the entity is a match, false otherwise</returns>
        public bool IsMatchNoFilter(Matcher match)
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return _IsMatchNoFilter(match);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        private bool _IsMatchNoFilter(Matcher match)
        {
            bool allOfMatch;
            bool anyOfMatch;
            bool noneOfMatch;

            allOfMatch = (match.AllOfTypeIndicies.Count > 0) ? match.AllOfTypeIndicies.All(_componentTypeIndiciesLookup.Contains) : true;
            anyOfMatch = (match.AnyOfTypeIndicies.Count > 0) ? match.AnyOfTypeIndicies.Intersect(_componentTypeIndicies).Any() : true;
            noneOfMatch = (match.NoneOfTypeIndicies.Count > 0) ? !match.NoneOfTypeIndicies.All(_componentTypeIndiciesLookup.Contains) : true;

            return allOfMatch && anyOfMatch && noneOfMatch;
        }


        /// <summary>
        /// Checks to see if this entity is a match with a Matcher only with
        /// using the filter part of a Matcher. Meaning it'll check to see if it
        /// passes the filter function but doesn't care about the components in 
        /// the entity. See <see cref="IsMatchNoFilter(Matcher)"/> for more information
        /// on why this is required.
        /// </summary>
        /// <param name="match">The <see cref="Matcher"/> to test the entity against</param>
        /// <returns>True if the entity is a match, false otherwise</returns>
        public bool IsMatchJustFilter(Matcher match)
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return (match.Filters.Count > 0) ? match.Filters.All(p => p(this)) : true;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Tests to see if the entity is a match with both in
        /// both its component composition and the Mathcer's filter.
        /// See <see cref="IsMatchNoFilter(Matcher)"/> and <see cref="IsMatchJustFilter(Matcher)"/>.
        /// </summary>
        /// <param name="match">The <see cref="Matcher"/> to test the entity against</param>
        /// <returns>True if the entity is a match, false otherwise</returns>
        public bool IsMatch(Matcher match)
        {
            _readerWriterLock.EnterReadLock();
            try { 
                return _IsMatchNoFilter(match) && IsMatchJustFilter(match);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        #endregion
    }
}
