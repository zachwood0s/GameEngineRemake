using ECS.Components;
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
        private HashSet<Entity> _observedEntitiesLookup;
        private List<Entity> _observedEntities;
        private Group _watchedGroup;

        public Watcher(Group groupToWatch)
        {
            _watchedGroup = groupToWatch;
            _observedEntitiesLookup = new HashSet<Entity>();
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
            if (!_observedEntitiesLookup.Contains(updatedEntity))
            {
                _observedEntities.Add(updatedEntity);
                _observedEntitiesLookup.Add(updatedEntity);
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
            _observedEntitiesLookup.Clear();
        }

        #region IEnumerable Implementation
        public IEnumerator<Entity> GetEnumerator() => _observedEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int EntityCount => _observedEntities.Count;

        public Entity this[int index]
        {
            get => _observedEntities[index]; 
            set => _observedEntities.Insert(index, value); 
        }

        #endregion
    }
}
