using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ECS
{
    public class Group: IEnumerable<Entity>
    {
        private List<Entity> _groupEntities;
        private Matcher _match;

        
        private event EntityChangedEventHandler _OnEntityComponentUpdated;
        private event EntityChangedEventHandler _OnEntityComponentRemoved;
        private event EntityChangedEventHandler _OnEntityComponentAdded;

        public Group(Matcher match)
        {
            _match = match;
            _groupEntities = new List<Entity>();
        }
        public void AddEntity(Entity entity)
        {
            if (!_groupEntities.Contains(entity))
            {
                entity.SubscribeToChanges(
                    _HandleEntityComponentUpdatedEvent,
                    _HandleEntityComponentRemovedEvent,
                    _HandleEntityComponentAddedEvent
                );
                _groupEntities.Add(entity);
            }
        }


        #region Subscribing / Unsubscribing

        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnEntityComponentUpdated += updated;
            _OnEntityComponentRemoved += removed;
            _OnEntityComponentAdded += added;
        }

        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnEntityComponentUpdated -= updated;
            _OnEntityComponentRemoved -= removed;
            _OnEntityComponentAdded -= added;
        }

        #endregion

        #region Handling Entity Changes

        private void _RemoveIfNotValid(Entity updatedEntity)
        {
            if (!updatedEntity.IsMatch(_match)) //Adding this component now made it invalid for this group
            {
                _groupEntities.Remove(updatedEntity);
                updatedEntity.UnSubscribeToChanges(
                    _HandleEntityComponentUpdatedEvent,
                    _HandleEntityComponentRemovedEvent,
                    _HandleEntityComponentAddedEvent
                );
            }
        }
        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _RemoveIfNotValid(updatedEntity);
            _OnEntityComponentAdded?.Invoke(updatedEntity, componentIndex, component);
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _RemoveIfNotValid(updatedEntity);
            _OnEntityComponentRemoved?.Invoke(updatedEntity, componentIndex, component);
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            //This will update any watchers we have 
            _OnEntityComponentUpdated?.Invoke(updatedEntity, componentIndex, component);
        }

        #endregion


        #region IEnumberable Implementation
        public IEnumerator<Entity> GetEnumerator()
        {
            return _groupEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity this[int index]
        {
            get { return _groupEntities[index]; }
            set { _groupEntities.Insert(index, value); }
        }
        public int EntityCount
        {
            get { return _groupEntities.Count; }
        }
        #endregion
    }
}
