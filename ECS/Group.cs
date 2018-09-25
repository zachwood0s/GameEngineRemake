using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using ECS.Matching;
using ECS.Entities;
using System.Collections.Concurrent;

namespace ECS
{
    public class Group: IEnumerable<Entity>
    {
        private SynchronizedCollection<Entity> _groupEntities;
        /// <summary>
        /// This is for all the entities that don't meet the filter requirements
        /// but still meet all the component requirements. We need to keep a list
        /// so that they can be added back when their filter passes again.
        /// </summary>
        private SynchronizedCollection<Entity> _cachedEntities; 
        protected Matcher _match;

        protected ReaderWriterLockSlim _readerWriterLock;
        
        private event EntityChangedEventHandler _OnEntityComponentUpdated;
        private event EntityChangedEventHandler _OnEntityComponentRemoved;
        private event EntityChangedEventHandler _OnEntityComponentAdded;

        private GroupBatchUpdater _groupBatchUpdater;

        public Group(Matcher match)
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _match = match;
            _groupEntities = new SynchronizedCollection<Entity>();
            _cachedEntities = new SynchronizedCollection<Entity>();
            _groupBatchUpdater = new GroupBatchUpdater(_RemoveAllNotValid, _groupEntities, _HandleEntityComponentUpdatedEvent);
        }
        public void AddEntity(Entity entity)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                if (!_groupEntities.Contains(entity))
                {
                    entity.SubscribeToChanges(
                        _HandleEntityComponentUpdatedEvent,
                        _HandleEntityComponentRemovedEvent,
                        _HandleEntityComponentAddedEvent
                    );

                    if (entity.IsMatch(_match)) //Entity matches filter and everything
                    {
                        _groupEntities.Add(entity);
                    }
                    else
                    {
                        //Otherwise we need to keep it for later when it could be updated
                        _AddToCached(entity);
                    }

                    _HandleEntityComponentAddedEvent(entity, 0, null);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
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
                _cachedEntities.Remove(removeEntity);
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

        protected void _RemoveOnlyBecauseFilter(Entity removeEntity)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _groupEntities.Remove(removeEntity);
                _AddToCached(removeEntity);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        protected void _AddBackIfCached(Entity updatedEntity)
        {
            if (_cachedEntities.Count > 0)
            {
                _readerWriterLock.EnterWriteLock();
                try
                {
                    if (_cachedEntities.Contains(updatedEntity))
                    {
                        _cachedEntities.Remove(updatedEntity);
                        _groupEntities.Add(updatedEntity);
                    }
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }
            }
        }

        private void _AddToCached(Entity entity)
        {
            if (!_cachedEntities.Contains(entity)){
                _cachedEntities.Add(entity);
            }
        }

        protected virtual void _RemoveAllNotValid()
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                Parallel.ForEach(_groupEntities, updatedEntity =>
                {
                    if (!updatedEntity.IsMatchNoFilter(_match))
                    {
                        _RemoveAndUnsubscribe(updatedEntity);
                    }
                    else if (!updatedEntity.IsMatchJustFilter(_match)) //Adding this component now made it invalid for this group
                    {
                        _RemoveOnlyBecauseFilter(updatedEntity);
                    }
                    else
                    {
                        _AddBackIfCached(updatedEntity);
                    }
                });
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
            
        }

        protected virtual bool _RemoveIfNotValid(Entity updatedEntity, bool componentAddedOrRemoved)
        {
            bool isValid = true;

            _readerWriterLock.EnterWriteLock();

            try
            {
                if (componentAddedOrRemoved && !updatedEntity.IsMatchNoFilter(_match))
                {
                    //We don't need to check if its a match without filter if the component was
                    //just updated and not removed/added. The filter will check if it's invalid because
                    //of an update
                    _RemoveAndUnsubscribe(updatedEntity);
                    isValid = false;
                }
                else if (!updatedEntity.IsMatchJustFilter(_match)) //Adding this component now made it invalid for this group
                {
                    _RemoveOnlyBecauseFilter(updatedEntity);
                    isValid = false;
                }
                else
                {
                    _AddBackIfCached(updatedEntity);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            return isValid;
        }

        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if (_RemoveIfNotValid(updatedEntity, true))
            {
                _OnEntityComponentAdded?.Invoke(updatedEntity, componentIndex, component);
            }
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if (_RemoveIfNotValid(updatedEntity, true))
            {
                _OnEntityComponentRemoved?.Invoke(updatedEntity, componentIndex, component);
            }
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            if(_match.Filters.Count == 0)
            {
                _OnEntityComponentUpdated?.Invoke(updatedEntity, componentIndex, component);
            }
            else if(_RemoveIfNotValid(updatedEntity, false))
            {
                _OnEntityComponentUpdated?.Invoke(updatedEntity, componentIndex, component);
            }
            /*
            if (_RemoveIfNotValid(updatedEntity, false))
            {
                //This will update any watchers we have 
                _OnEntityComponentUpdated?.Invoke(updatedEntity, componentIndex, component);
            }
            */
        }

        internal void SceneRemovedEntity(Entity e)
        {
            _RemoveAndUnsubscribe(e);
        }
        public void ApplyToAllEntities(Action<Entity> action)
        {
            _readerWriterLock.EnterWriteLock();
            for(int i = 0; i<_groupEntities.Count; i++)
            {
                action(_groupEntities[i]);
            }
            _readerWriterLock.ExitWriteLock();
        }

        public void UpdateAllEntitiesInGroup<T>(Action<Entity, T> updateAction) where T: class, IComponent
        {
            _groupBatchUpdater.UpdateAllEntities(updateAction);
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
        public int CachedEntityCount => _cachedEntities.Count;

        #endregion

        private class GroupBatchUpdater
        {
            private SynchronizedCollection<Entity> _entities;
            private ConcurrentDictionary<Entity, EntityChangedEventHandler> _OnComponentUpdatedEvents;
            private readonly Action _removalAction;
            private readonly EntityChangedEventHandler _groupUpdateEvent;

            public GroupBatchUpdater(Action removalAction, SynchronizedCollection<Entity> entities, EntityChangedEventHandler groupUpdateEvent)
            {
                _removalAction = removalAction;
                _entities = entities;
                _groupUpdateEvent = groupUpdateEvent;
                _OnComponentUpdatedEvents = new ConcurrentDictionary<Entity, EntityChangedEventHandler>();
            }

            public void UpdateAllEntities<T>(Action<Entity, T> updateAction) where T: class, IComponent
            {
                Parallel.ForEach(_entities, entity =>
                {
                    T component = entity.GetComponent<T>();
                    int index = ComponentPool.GetComponentIndex<T>();
                    updateAction(entity, component);
                    _CallEntityUpdateEvent(entity, index, component);
                });
                _removalAction();
            }

            private void _CallEntityUpdateEvent(Entity entity, int componentIndex, IComponent component)
            {
                if(!_OnComponentUpdatedEvents.TryGetValue(entity, out EntityChangedEventHandler updateEvent))
                {
                    EntityChangedEventHandler eventHandler = entity.OnComponentUpdated - _groupUpdateEvent;
                    _OnComponentUpdatedEvents.GetOrAdd(entity, eventHandler);
                }
                updateEvent?.Invoke(entity, componentIndex, component);
            }
        }
    }
}
