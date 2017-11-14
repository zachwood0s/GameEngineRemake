using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

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

        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _observedEntities.Add(updatedEntity);
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _observedEntities.Add(updatedEntity);
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _observedEntities.Add(updatedEntity);
        }

        public void ClearObservedEntities()
        {
            _observedEntities.Clear();
        }

        #region IEnumerable Implementation
        public IEnumerator<Entity> GetEnumerator()
        {
            return _observedEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity this[int index]
        {
            get { return _observedEntities[index]; }
            set { _observedEntities.Insert(index, value); }
        }
        public int EntityCount
        {
            get { return _observedEntities.Count; }
        }

        #endregion
    }
}
