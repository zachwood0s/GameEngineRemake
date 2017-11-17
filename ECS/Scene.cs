using ECS.Components;
using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Scene
    {
        private Dictionary<Matcher, Group> _groups;
        private List<List<Group>> _groupsForIndex;
        private List<Entity> _entities;
        private SystemPool _systemPool;

        public Scene()
        {
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
            _entities.Add(entity);
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
            if (!_groups.ContainsKey(match))
            {
                _groups.Add(match, _CreateGroup(match));
            }

            return _groups[match];
        }

        private void _AddToExistingGroups(Entity entity)
        {
            foreach(KeyValuePair<Matcher, Group> pair in _groups)
            {
                if (entity.IsMatch(pair.Key))
                {
                    pair.Value.AddEntity(entity);
                }
            }
        }
        private Group _CreateGroup(Matcher match)
        {
            Group newGroup = new Group(match);

            foreach(Entity entity in _entities)
            {
                if (entity.IsMatch(match))
                {
                    newGroup.AddEntity(entity);
                }
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
