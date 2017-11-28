using ECS.Components;
using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ECS
{
    public class Scene
    {
        private Dictionary<Matcher, Group> _groups;
        private List<List<Group>> _groupsForIndex;
        private List<Entity> _entities;
        private SystemPool _systemPool;

        private ReaderWriterLockSlim _readerWriterLock; 

        public Scene()
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _systemPool = new SystemPool();
            _entities = new List<Entity>();
            _groupsForIndex = new List<List<Group>>();
            _groups = new Dictionary<Matcher, Group>(); 
        }

        #region Getters/Setters

        public SystemPool SystemPool => _systemPool;

        #endregion

        #region Systems

        public void Initialize()
        {
            _systemPool.Initialize();
        }
        public void Execute()
        {
            _systemPool.Execute();
        }
        public void CleanUp()
        {

        }

        #endregion

        public Entity CreateEntity()
        {
            Entity entity = new Entity();

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
                    if (entity.IsMatch(pair.Key))
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
                    if (entity.IsMatch(match))
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
