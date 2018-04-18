using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ECS
{
    public class Group: IEnumerable<Entity>
    {
        private List<Entity> _groupEntities;
        protected Matcher _match;

        protected ReaderWriterLockSlim _readerWriterLock;
        
        private event EntityChangedEventHandler _OnEntityComponentUpdated;
        private event EntityChangedEventHandler _OnEntityComponentRemoved;
        private event EntityChangedEventHandler _OnEntityComponentAdded;

        public Group(Matcher match)
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _match = match;
            _groupEntities = new List<Entity>();
        }
        public void AddEntity(Entity entity)
        {
            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (!_groupEntities.Contains(entity))
                {
                    entity.SubscribeToChanges(
                        _HandleEntityComponentUpdatedEvent,
                        _HandleEntityComponentRemovedEvent,
                        _HandleEntityComponentAddedEvent
                    );

                    _readerWriterLock.EnterWriteLock();

                    try
                    {
                        _groupEntities.Add(entity);
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock();
                    }

                    _HandleEntityComponentAddedEvent(entity, 0, null);
                }
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }


        #region Subscribing / Unsubscribing

        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _OnEntityComponentUpdated += updated;
                _OnEntityComponentRemoved += removed;
                _OnEntityComponentAdded += added;
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
                _OnEntityComponentUpdated -= updated;
                _OnEntityComponentRemoved -= removed;
                _OnEntityComponentAdded -= added;
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        #endregion

        #region Handling Entity Changes

        protected void _RemoveAndUnsubscribe(Entity removeEntity)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _groupEntities.Remove(removeEntity);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            removeEntity.UnSubscribeToChanges(
                _HandleEntityComponentUpdatedEvent,
                _HandleEntityComponentRemovedEvent,
                _HandleEntityComponentAddedEvent
            );
        }

        protected virtual bool _RemoveIfNotValid(Entity updatedEntity)
        {
            bool isValid = true;

            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (!updatedEntity.IsMatch(_match)) //Adding this component now made it invalid for this group
                {
                    _RemoveAndUnsubscribe(updatedEntity);
                    isValid = false;
                }
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }

            return isValid;
        }
        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if (_RemoveIfNotValid(updatedEntity))
            {
                _OnEntityComponentAdded?.Invoke(updatedEntity, componentIndex, component);
            }
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if (_RemoveIfNotValid(updatedEntity))
            {
                _OnEntityComponentRemoved?.Invoke(updatedEntity, componentIndex, component);
            }
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if (_RemoveIfNotValid(updatedEntity))
            {
                //This will update any watchers we have 
                _OnEntityComponentUpdated?.Invoke(updatedEntity, componentIndex, component);
            }
        }

        #endregion

        #region IEnumberable Implementation

        public IEnumerator<Entity> GetEnumerator() => _groupEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Entity this[int index]
        {
            get => _groupEntities[index]; 
            set => _groupEntities.Insert(index, value); 
        }
        public int EntityCount => _groupEntities.Count;

        #endregion
    }
}
