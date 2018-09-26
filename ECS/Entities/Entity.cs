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
        /// NOT THREAD SAFE!
        /// </summary>
        public IReadOnlyList<IComponent> Components => _components;
        /// <summary>
        /// NOT THREAD SAFE!
        /// </summary>
        public IReadOnlyList<int> ComponentTypeIndicies => _componentTypeIndicies;

        #endregion

        #region Subscribing/Unsubscribing

        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _readerWriterLock.EnterWriteLock();
            _OnComponentUpdated += updated;
            _OnComponentRemoved += removed;
            _OnComponentAdded += added;
            _readerWriterLock.ExitWriteLock();
        }

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
        public void GetComponent<T1, T2>(Action<T1, T2> getAction) where T1: class, IComponent
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
        public void GetComponent<T1, T2, T3>(Action<T1, T2, T3> getAction) where T1: class, IComponent
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
        public void GetComponent<T1, T2, T3, T4>(Action<T1, T2, T3, T4> getAction) where T1: class, IComponent
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
        public void GetComponent<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> getAction) where T1: class, IComponent
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

                /*
                if (newComp is SettableComponent maybeSettable)
                {
                    maybeSettable.SubscribeToChanges(SettableComponentUpdated);
                }
                */
                _OnComponentAdded?.Invoke(this, newCompIndex, newComp);
            }
        }
        #endregion

        #region Removing Components

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

            /*
            SettableComponent maybeSettable = oldComp as SettableComponent;
            if (maybeSettable != null)
            {
                maybeSettable.UnSubscribeToChanges(SettableComponentUpdated);
            }

            */
            _OnComponentRemoved?.Invoke(this, oldCompIndex, oldComp);
        }

        #endregion

        #region Updating Components

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
                //throw new UnregisteredComponentException($"Component of type {typeof(T).Name} has not been added to entity and is trying to be replaced.");

            _components[existingCompIndex] = component;
            _OnComponentUpdated(this, newCompPoolIndex, component);
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
        //Only 5 cuz I don't feel like going farther. If there's a need I will
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

        /*
        public void SettableComponentUpdated(int componentIndex, IComponent component)
        {
            _OnComponentUpdated?.Invoke(this, componentIndex, component); 
        }
        */


        #endregion

        #region Matching

        //I Don't really like how this looks
        //I'm not sure it makes a lot of sense for the groupmanager
        //to handle the matching side of things
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

        public bool IsMatchJustFilter(Matcher match)
        {
            _readerWriterLock.EnterReadLock();
            bool result;
            try
            {
                return (match.Filters.Count > 0) ? match.Filters.All(p => p(this)) : true;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public bool IsMatch(Matcher match)
        {
            //I think the lock is still necessary because the IsMatch
            //Function in the groupManager will iterate through the Component Lists

            _readerWriterLock.EnterReadLock();
            try { 
                bool filterMatch;

                filterMatch = (match.Filters.Count > 0) ? match.Filters.All(p => p(this)) : true;

                return _IsMatchNoFilter(match) && filterMatch;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        #endregion
    }
}
