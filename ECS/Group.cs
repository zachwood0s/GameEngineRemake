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

namespace ECS
{
    public class Group: IEnumerable<Entity>
    {
        private List<Entity> _groupEntities;
        /// <summary>
        /// This is for all the entities that don't meet the filter requirements
        /// but still meet all the component requirements. We need to keep a list
        /// so that they can be added back when their filter passes again.
        /// </summary>
        private List<Entity> _cachedEntities; 
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
            _cachedEntities = new List<Entity>();
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
                        if (entity.IsMatch(_match)) //Entity matches filter and everything
                        {
                            _groupEntities.Add(entity);
                        }
                        else
                        {
                            //Otherwise we need to keep it for later when it could be updated
                            _AddToCached(entity);
                        }
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
            _readerWriterLock.EnterUpgradeableReadLock();
            try
            {
                if (_cachedEntities.Contains(updatedEntity))
                {
                    _readerWriterLock.EnterWriteLock();
                    try
                    {
                        _cachedEntities.Remove(updatedEntity);
                        _groupEntities.Add(updatedEntity);
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        private void _AddToCached(Entity entity)
        {
            if (!_cachedEntities.Contains(entity)){
                _cachedEntities.Add(entity);
            }
        }

        protected virtual bool _RemoveIfNotValid(Entity updatedEntity)
        {
            bool isValid = true;

            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (!updatedEntity.IsMatchNoFilter(_match))
                {
                    _RemoveAndUnsubscribe(updatedEntity);
                    isValid = false;
                }
                else if (!updatedEntity.IsMatch(_match)) //Adding this component now made it invalid for this group
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

        internal void SceneRemovedEntity(Entity e)
        {
            _RemoveAndUnsubscribe(e);
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
    }
}
