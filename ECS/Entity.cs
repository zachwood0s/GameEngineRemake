using ECS.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public delegate void EntityChangedEventHandler(Entity entity, int componentPoolIndex, IComponent component);
    public class Entity
    {
        private List<IComponent> _components;
        private List<int> _componentTypeIndicies;

        private event EntityChangedEventHandler _OnComponentUpdated;
        private event EntityChangedEventHandler _OnComponentRemoved;
        private event EntityChangedEventHandler _OnComponentAdded;

        public Entity()
        {
            _components = new List<IComponent>();
            _componentTypeIndicies = new List<int>();
        }

        #region Getters/Setters
        public List<IComponent> Components
        {
            get { return _components; }
        }
        #endregion

        #region Component Checking
        public T GetComponent<T>() where T : class, IComponent
        {
            foreach(IComponent comp in _components)
            {
                if(comp is T)
                {
                    return (T) comp;
                }
            }
            return null;
        }
        public bool HasComponent<T>() where T: class, IComponent
        {
            return _components.Exists(c => c is T);
        }
        public bool HasComponent(int componentPoolIndex)
        {
            return _componentTypeIndicies.Contains(componentPoolIndex);
        }
        #endregion


        #region Subscribing/Unscubscribing To Changes
        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnComponentUpdated += updated;
            _OnComponentRemoved += removed;
            _OnComponentAdded += added;
        }

        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnComponentUpdated -= updated;
            _OnComponentRemoved -= removed;
            _OnComponentAdded -= added;
        }

        #endregion

        #region Adding Components
        public Entity With(IComponent newComp)
        {
            int newCompIndex = ComponentPool.GetComponentIndex(newComp.GetType());
            if (!_componentTypeIndicies.Contains(newCompIndex))
            {
                _components.Add(newComp);
                _componentTypeIndicies.Add(newCompIndex);

                _OnComponentAdded?.Invoke(this, newCompIndex, newComp);
            }
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
            int compIndex = _componentTypeIndicies.IndexOf(oldCompIndex);
            if (compIndex > -1)
            {
                IComponent oldComp = _components[compIndex];
                _components.RemoveAt(compIndex);
                _componentTypeIndicies.RemoveAt(compIndex);

                _OnComponentRemoved?.Invoke(this, oldCompIndex, oldComp);
                
            }
        }

        #endregion

        #region Updating Components

        public void UpdateComponent<T>(T component) where T:class, IComponent
        {
            int newCompPoolIndex = ComponentPool.GetComponentIndex<T>();
            int existingCompIndex = _componentTypeIndicies.IndexOf(newCompPoolIndex);
            if (existingCompIndex > -1)
            {
                _components[existingCompIndex] = component;

                _OnComponentUpdated?.Invoke(this, newCompPoolIndex, component);
            }
            else
            {
                Debug.WriteLine("Warning: Attempting to replace component that doesn't exist");
            }
           
        }


        #endregion

        #region Matching

        public bool IsMatch(Matcher match)
        {
            bool allOfMatch = (match.AllOfTypeIndicies.Count > 0) ? match.AllOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;
            bool anyOfMatch = (match.AnyOfTypeIndicies.Count > 0) ? match.AnyOfTypeIndicies.Intersect(_componentTypeIndicies).Any() : true;
            bool noneOfMatch = (match.NoneOfTypeIndicies.Count > 0) ? !match.NoneOfTypeIndicies.All(_componentTypeIndicies.Contains) : true;

            return allOfMatch && anyOfMatch && noneOfMatch;
        }

        #endregion
    }
}
