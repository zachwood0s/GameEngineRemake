﻿using ECS.Components;
using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ECS.Matching;
using ECS.Entities;
using ECS.Systems.SystemPools;

namespace ECS
{
    public delegate Entity EntityFactoryMethod();
    public class Scene
    {
        public static EntityFactoryMethod EntityFactoryMethod = () => new Entity();

        private Dictionary<Matcher, Group> _groups;
        private List<List<Group>> _groupsForIndex;
        private List<Entity> _entities;

        private List<SystemPool> _systemPools;

        private ReaderWriterLockSlim _readerWriterLock; 

        public Scene()
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _systemPools = new List<SystemPool>();
            _entities = new List<Entity>();
            _groupsForIndex = new List<List<Group>>();
            _groups = new Dictionary<Matcher, Group>(); 
        }

        #region Getters/Setters

        public int EntityCount => _entities.Count;
        public IReadOnlyList<SystemPool> SystemPools => _systemPools;

        #endregion

        #region Systems

        public void Initialize()
        {
            foreach (SystemPool pool in _systemPools)
            {
                pool.Initialize();
            }
        }
        public void Execute()
        {
            foreach (SystemPool pool in _systemPools)
            {
                pool.Execute();
            }
        }
        public void CleanUp()
        {
            foreach (SystemPool pool in _systemPools)
            {
                pool.CleanUp();
            }
        }

        public void AddSystemPool(SystemPool pool) => _systemPools.Add(pool);

        public SystemPool AddSystemPoolFromBuilder(SystemPoolBuilder builder, string suffix = "")
        {
            SystemPool pool = builder.Build(this, suffix);
            AddSystemPool(pool);
            return pool;
        }

        public SystemPool GetSystemPoolByName(string name)
        {
            return _systemPools.Find(p => p.PoolName == name);
        }

        #endregion

        #region Entities

        public Entity CreateEntity()
        {
            Entity entity = EntityFactoryMethod();

            _readerWriterLock.EnterWriteLock();

            try
            {
                _entities.Add(entity);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            entity.SubscribeToChanges(
                _HandleEntityComponentUpdatedEvent, 
                _HandleEntityComponentRemovedEvent, 
                _HandleEntityComponentAddedEvent
            );
            return entity;
        }

        public bool RemoveEntity(Entity entity)
        {
            _readerWriterLock.EnterWriteLock();

            bool wasRemoved;

            try
            {
                wasRemoved = _entities.Remove(entity);
                foreach (KeyValuePair<Matcher, Group> pair in _groups)
                {
                    pair.Value.SceneRemovedEntity(entity);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
            if (wasRemoved)
            {
                entity.UnSubscribeToChanges(
                    _HandleEntityComponentUpdatedEvent,
                    _HandleEntityComponentRemovedEvent,
                    _HandleEntityComponentAddedEvent
                );
                return true;
            }

            return false;
        }

        public Entity CreateEntityFromBuilder(IEntityBuilder builder)
        {
            return builder.Build(this);
        }

        public void AddEntity(Entity newEntity)
        {
            _readerWriterLock.EnterWriteLock();
            try
            {
                _entities.Add(newEntity);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            newEntity.SubscribeToChanges(
                _HandleEntityComponentUpdatedEvent, 
                _HandleEntityComponentRemovedEvent, 
                _HandleEntityComponentAddedEvent
            );

            _AddToExistingGroups(newEntity);
        }

        #endregion

        #region Groups

        public Group GetGroup(Matcher match)
        {
            _readerWriterLock.EnterUpgradeableReadLock();

            Group returnGroup;

            try
            {
                if (!_groups.ContainsKey(match))
                {
                    _readerWriterLock.EnterWriteLock();

                    try
                    {
                        _groups.Add(match, _CreateGroup(match));
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock(); 
                    }
                }
                returnGroup = _groups[match];
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }

            return returnGroup;
        }

        private void _AddToExistingGroups(Entity entity)
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                foreach (KeyValuePair<Matcher, Group> pair in _groups)
                {
                    if (entity.IsMatchNoFilter(pair.Key))
                    {
                        pair.Value.AddEntity(entity);
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        private Group _CreateGroup(Matcher match)
        {
            Group newGroup = new Group(match);

            _readerWriterLock.EnterReadLock();

            try
            {
                foreach (Entity entity in _entities)
                {
                    if (entity.IsMatchNoFilter(match))
                    {
                        newGroup.AddEntity(entity);
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return newGroup;
        }

        #endregion


        private void _HandleEntityComponentAddedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            _AddToExistingGroups(updatedEntity);
        }
        private void _HandleEntityComponentRemovedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            
        }
        private void _HandleEntityComponentUpdatedEvent(Entity updatedEntity, int componentIndex, IComponent component)
        {
            
        }
    }
}
