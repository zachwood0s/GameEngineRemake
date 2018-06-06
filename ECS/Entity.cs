using ECS.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ECS.Components.Exceptions;
using ECS.EntityGroupManager;
using ECS.Matching;

namespace ECS
{
    public delegate void EntityChangedEventHandler(Entity entity, int componentPoolIndex, IComponent component);
    public class Entity
    {
        private IEntityGroupManager _groupManager;
        
        private ReaderWriterLockSlim _readerWriterLock;
        private List<IComponent> _components;
        private List<int> _componentTypeIndicies;

        //private event EntityChangedEventHandler _OnComponentUpdated;
        //private event EntityChangedEventHandler _OnComponentRemoved;
        //private event EntityChangedEventHandler _OnComponentAdded;

        public Entity(IEntityGroupManager groupManager)
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _components = new List<IComponent>();
            _componentTypeIndicies = new List<int>();
            _groupManager = groupManager;
        }

        #region Getters/Setters

        /// <summary>
        /// NOT THREAD SAFE!
        /// </summary>
        public IReadOnlyList<IComponent> Components => _components;
        /// <summary>
        /// NOT THREAD SAFE!
        /// </summary>
        public IReadOnlyList<int> ComponentTypeIndicies => _componentTypeIndicies;

        #endregion

        #region Component Checking

        public T GetComponent<T>() where T : class, IComponent
        {
            T returnComp = null;
            _readerWriterLock.EnterReadLock();

            try
            {
                foreach (IComponent comp in _components)
                {
                    if (comp is T)
                    {
                        returnComp = (T)comp;
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            return returnComp;
        }

        public bool HasComponent<T>() where T: class, IComponent
        {
            _readerWriterLock.EnterReadLock();
            bool doesExist;
            try
            {
                doesExist = _components.Exists(c => c is T);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return doesExist; 
        }

        public bool HasComponent(int componentPoolIndex)
        {
            _readerWriterLock.EnterReadLock();
            bool doesExist;
            try
            {
                doesExist = _componentTypeIndicies.Contains(componentPoolIndex);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return doesExist;
        }

        #endregion

        #region Subscribing/Unscubscribing To Changes

        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _groupManager.SubscribeToChanges(updated, removed, added);
            /*
            _readerWriterLock.EnterWriteLock();

            try
            {
                _OnComponentUpdated += updated;
                _OnComponentRemoved += removed;
                _OnComponentAdded += added;
            }
            finally
            {
            //    _readerWriterLock.ExitWriteLock();
            }*/
        }

        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _groupManager.UnSubscribeToChanges(updated, removed, added);
            /*
            _readerWriterLock.EnterWriteLock();

            try
            {
                _OnComponentUpdated -= updated;
                _OnComponentRemoved -= removed;
                _OnComponentAdded -= added;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }*/
        }

        #endregion

        #region Adding Components
        public Entity With(IComponent newComp)
        {
            int newCompIndex = ComponentPool.GetComponentIndex(newComp.GetType());

            if (newCompIndex < 0)
                throw new UnregisteredComponentException($"Component of type {newComp.GetType()} has not been registered with the ComponentPool.");

            _readerWriterLock.EnterUpgradeableReadLock();

            if (!_componentTypeIndicies.Contains(newCompIndex))
            {
                _readerWriterLock.EnterWriteLock();

                _components.Add(newComp);
                _componentTypeIndicies.Add(newCompIndex);

                /*
                if (newComp is SettableComponent maybeSettable)
                {
                    maybeSettable.SubscribeToChanges(SettableComponentUpdated);
                }
                */

                _readerWriterLock.ExitWriteLock();

                _groupManager.EntityAddedComponent(this, newCompIndex, newComp);
                //_OnComponentAdded?.Invoke(this, newCompIndex, newComp);
            }

            _readerWriterLock.ExitUpgradeableReadLock();

            return this;
        }
        public Entity With<T>() where T : IComponentHasDefault
        {
            T newComp = Activator.CreateInstance<T>();
            newComp.SetDefaults();
            return With(newComp);
        }
        #endregion

        #region Removing Components

        public Entity Remove(Type oldCompType)
        {
            int oldCompIndex = ComponentPool.GetComponentIndex(oldCompType);
            _Remove(oldCompIndex);
            return this;
        }
        public Entity Remove(IComponent oldComp)
        {
            return Remove(oldComp.GetType());
        }

        public Entity Remove<T>() where T : IComponent
        {
            int oldCompIndex = ComponentPool.GetComponentIndex<T>();
            _Remove(oldCompIndex);
            return this;
        }

        private void _Remove(int oldCompIndex)
        {
            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                int compIndex = _componentTypeIndicies.IndexOf(oldCompIndex);

                if(compIndex < 0)
                    throw new UnregisteredComponentException($"Component of type " +
                        $"{ComponentPool.Components[oldCompIndex].GetType()} has not been added to the current entity and is trying to be removed.");

                _readerWriterLock.EnterWriteLock();
                IComponent oldComp;
                try
                {
                    oldComp = _components[compIndex];
                    _components.RemoveAt(compIndex);
                    _componentTypeIndicies.RemoveAt(compIndex);
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }

                /*
                SettableComponent maybeSettable = oldComp as SettableComponent;
                if (maybeSettable != null)
                {
                    maybeSettable.UnSubscribeToChanges(SettableComponentUpdated);
                }

                */

                _groupManager.EntityRemovedComponent(this, oldCompIndex, oldComp);
                //_OnComponentRemoved?.Invoke(this, oldCompIndex, oldComp);
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        #endregion

        #region Updating Components

        public void UpdateComponent<T>(T component) where T:class, IComponent
        {
            int newCompPoolIndex = ComponentPool.GetComponentIndex<T>();

            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                int existingCompIndex = _componentTypeIndicies.IndexOf(newCompPoolIndex);

                if (existingCompIndex < 0)
                    throw new UnregisteredComponentException($"Component of type {typeof(T).GetType()} has not been added to entity and is trying to be replaced.");

                _readerWriterLock.EnterWriteLock();

                try
                {
                    _components[existingCompIndex] = component;
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }

                _groupManager.EntityUpdatedComponent(this, newCompPoolIndex, component);
                //_OnComponentUpdated?.Invoke(this, newCompPoolIndex, component);
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
           
        }

        /*
         * TODO:
         * I think these functions could be sped up every so slightly by not
         * actually calling the UpdateComponent() base method because it does
         * other things that these functions have already guaranteed. (ex: 
         * it checks whether the component is already in the entity, which in
         * these cases we already know it is.) We should be able to just notify
         * all of our watchers
         */
        public void UpdateComponent<T>(Action<T> updateAction) where T: class, IComponent
        {
            T comp = GetComponent<T>();
            if (comp != null)
            {
                updateAction(comp);
                UpdateComponent(comp);
            }
        }
        //Only 5 cuz I don't feel like going farther. If there's a need I will
        public void UpdateComponents<T1, T2>(Action<T1, T2> updateAction) where T1: class, IComponent 
                                                                          where T2: class, IComponent
        {
            T1 c1 = GetComponent<T1>(); T2 c2 = GetComponent<T2>();
            if (c1 != null && c2 != null)
            {
                updateAction(c1, c2);
                UpdateComponent(c1); UpdateComponent(c2);
            }
        }
        public void UpdateComponents<T1, T2, T3>(Action<T1, T2, T3> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
        {
            T1 c1 = GetComponent<T1>(); T2 c2 = GetComponent<T2>(); T3 c3 = GetComponent<T3>();
            if (c1 != null && c2 != null && c3 != null)
            {
                updateAction(c1, c2, c3);
                UpdateComponent(c1); UpdateComponent(c2); UpdateComponent(c3);
            }
        }
        public void UpdateComponents<T1, T2, T3, T4>(Action<T1, T2, T3, T4> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
                                                                                  where T4: class, IComponent
        {
            T1 c1 = GetComponent<T1>(); T2 c2 = GetComponent<T2>(); T3 c3 = GetComponent<T3>(); T4 c4 = GetComponent<T4>();
            if (c1 != null && c2 != null && c3 != null && c4 != null)
            {
                updateAction(c1, c2, c3, c4);
                UpdateComponent(c1); UpdateComponent(c2); UpdateComponent(c3); UpdateComponent(c4);
            }
        }
        public void UpdateComponents<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> updateAction) where T1: class, IComponent 
                                                                                  where T2: class, IComponent
                                                                                  where T3: class, IComponent
                                                                                  where T4: class, IComponent
                                                                                  where T5: class, IComponent
        {
            T1 c1 = GetComponent<T1>(); T2 c2 = GetComponent<T2>(); T3 c3 = GetComponent<T3>(); T4 c4 = GetComponent<T4>(); T5 c5 = GetComponent<T5>();
            if (c1 != null && c2 != null && c3 != null && c4 != null && c5 != null)
            {
                updateAction(c1, c2, c3, c4, c5);
                UpdateComponent(c1); UpdateComponent(c2); UpdateComponent(c3); UpdateComponent(c4); UpdateComponent(c5);
            }
        }

        /*
        public void SettableComponentUpdated(int componentIndex, IComponent component)
        {
            _OnComponentUpdated?.Invoke(this, componentIndex, component); 
        }
        */


        #endregion

        #region Matching

        public bool IsMatchNoFilter(Matcher match)
        {
            _readerWriterLock.EnterReadLock();
            bool result;
            try
            {
                result = _groupManager.IsMatchNoFilter(match, this);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            return result;
        }

        public bool IsMatch(Matcher match)
        {
            //I think the lock is still necessary because the IsMatch
            //Function in the groupManager will iterate through the Component Lists

            _readerWriterLock.EnterReadLock();
            bool result;
            try
            {
                result = _groupManager.IsMatch(match, this);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            return result;
/*
            bool allOfMatch;
            bool anyOfMatch;
            bool noneOfMatch;
            bool filterMatch;

            try
            {
                allOfMatch = (match.AllOfTypeIndicies.Count > 0) ? match.AllOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;
                anyOfMatch = (match.AnyOfTypeIndicies.Count > 0) ? match.AnyOfTypeIndicies.Intersect(_componentTypeIndicies).Any() : true;
                noneOfMatch = (match.NoneOfTypeIndicies.Count > 0) ? !match.NoneOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;
                filterMatch = (match.Filters.Count > 0) ? match.Filters.All(p => p(this)) : true;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return allOfMatch && anyOfMatch && noneOfMatch && filterMatch;
            */
        }

        #endregion
    }
}
