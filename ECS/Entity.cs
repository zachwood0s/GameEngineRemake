using ECS.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ECS
{
    public delegate void EntityChangedEventHandler(Entity entity, int componentPoolIndex, IComponent component);
    public class Entity
    {

        
        private ReaderWriterLockSlim _readerWriterLock;
        private List<IComponent> _components;
        private List<int> _componentTypeIndicies;

        private event EntityChangedEventHandler _OnComponentUpdated;
        private event EntityChangedEventHandler _OnComponentRemoved;
        private event EntityChangedEventHandler _OnComponentAdded;

        public Entity()
        {
            _readerWriterLock = new ReaderWriterLockSlim();
            _components = new List<IComponent>();
            _componentTypeIndicies = new List<int>();
        }

        #region Getters/Setters

        /// <summary>
        /// NOT THREAD SAVE I DON'T THINK
        /// </summary>
        public List<IComponent> Components => _components;

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
            _readerWriterLock.EnterWriteLock();

            try
            {
                _OnComponentUpdated += updated;
                _OnComponentRemoved += removed;
                _OnComponentAdded += added;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
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
            }
        }

        #endregion

        #region Adding Components
        public Entity With(IComponent newComp)
        {
            int newCompIndex = ComponentPool.GetComponentIndex(newComp.GetType());

            _readerWriterLock.EnterUpgradeableReadLock();

            if (!_componentTypeIndicies.Contains(newCompIndex))
            {
                _readerWriterLock.EnterWriteLock();

                _components.Add(newComp);
                _componentTypeIndicies.Add(newCompIndex);

                SettableComponent maybeSettable = newComp as SettableComponent;
                if(maybeSettable != null)
                {
                    maybeSettable.SubscribeToChanges(SettableComponentUpdated);
                }

                _readerWriterLock.ExitWriteLock();

                _OnComponentAdded?.Invoke(this, newCompIndex, newComp);
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
            int oldCompIndex = ComponentPool.GetComponentIndex(oldComp.GetType());
            _Remove(oldCompIndex);
            return this;
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
                if (compIndex > -1)
                {
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

                    SettableComponent maybeSettable = oldComp as SettableComponent;
                    if (maybeSettable != null)
                    {
                        maybeSettable.UnSubscribeToChanges(SettableComponentUpdated);
                    }

                    _OnComponentRemoved?.Invoke(this, oldCompIndex, oldComp);
                }
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
                if (existingCompIndex > -1)
                {
                    _readerWriterLock.EnterWriteLock();

                    try
                    {
                        _components[existingCompIndex] = component;
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock();
                    }

                    _OnComponentUpdated?.Invoke(this, newCompPoolIndex, component);
                }
                else
                {
                    Debug.WriteLine("Warning: Attempting to replace component that doesn't exist");
                }
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
           
        }

        public void SettableComponentUpdated(int componentIndex, IComponent component)
        {
            _OnComponentUpdated?.Invoke(this, componentIndex, component); 
        }


        #endregion

        #region Matching

        public bool IsMatch(Matcher match)
        {
            _readerWriterLock.EnterReadLock();

            bool allOfMatch;
            bool anyOfMatch;
            bool noneOfMatch;

            try
            {
                allOfMatch = (match.AllOfTypeIndicies.Count > 0) ? match.AllOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;
                anyOfMatch = (match.AnyOfTypeIndicies.Count > 0) ? match.AnyOfTypeIndicies.Intersect(_componentTypeIndicies).Any() : true;
                noneOfMatch = (match.NoneOfTypeIndicies.Count > 0) ? !match.NoneOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return allOfMatch && anyOfMatch && noneOfMatch;
        }

        #endregion
    }
}
