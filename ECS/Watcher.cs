﻿using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ECS.Entities;

namespace ECS
{
    public class Watcher:IEnumerable<Entity>
    {
        private List<Entity> _observedEntities;
        private Group _watchedGroup;

        public Watcher(Group groupToWatch)
        {
            _watchedGroup = groupToWatch;
            _observedEntities = new List<Entity>();
            _watchedGroup.SubscribeToChanges(
                _HandleEntityComponentUpdatedEvent,
                _HandleEntityComponentRemovedEvent,
                _HandleEntityComponentAddedEvent
            );
        }

        public void Enable()
        {

        }
        public void Disable()
        {

        }
        private void _AddToObserved(Entity updatedEntity)
        {
            if (!_observedEntities.Contains(updatedEntity))
            {
                _observedEntities.Add(updatedEntity);
            }
        }

        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _AddToObserved(updatedEntity);
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _AddToObserved(updatedEntity);
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _AddToObserved(updatedEntity);
        }

        public void ClearObservedEntities()
        {
            _observedEntities.Clear();
        }

        #region IEnumerable Implementation
        public IEnumerator<Entity> GetEnumerator() => _observedEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Entity this[int index]
        {
            get => _observedEntities[index]; 
            set => _observedEntities.Insert(index, value); 
        }
        public int EntityCount => _observedEntities.Count;

        #endregion
    }
}
