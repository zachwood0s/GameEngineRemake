using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS.Components;
using ECS.Matching;
using ECS.Entities;

namespace ECS.EntityGroupManager
{
    internal class EntityGroupManager : IEntityGroupManager
    {
        private event EntityChangedEventHandler _OnComponentUpdated;
        private event EntityChangedEventHandler _OnComponentRemoved;
        private event EntityChangedEventHandler _OnComponentAdded;

        public void EntityAddedComponent(Entity entity, int componentIndex, IComponent component)
        {
            _OnComponentAdded.Invoke(entity, componentIndex, component);
        }

        public void EntityRemovedComponent(Entity entity, int componentIndex, IComponent component)
        {
            _OnComponentRemoved.Invoke(entity, componentIndex, component);
        }

        public void EntityUpdatedComponent(Entity entity, int componentIndex, IComponent component)
        {
            _OnComponentUpdated.Invoke(entity, componentIndex, component);
        }

        public void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnComponentUpdated += updated;
            _OnComponentRemoved += removed;
            _OnComponentAdded += added;
        }

        public void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added)
        {
            _OnComponentUpdated -= updated;
            _OnComponentRemoved -= removed;
            _OnComponentAdded -= added;
        }

        public bool IsMatch(Matcher match, Entity entity)
        {
            bool filterMatch;

            filterMatch = (match.Filters.Count > 0) ? match.Filters.All(p => p(entity)) : true;

            return IsMatchNoFilter(match, entity) && filterMatch;
        }

        public bool IsMatchNoFilter(Matcher match, Entity entity)
        {
            bool allOfMatch;
            bool anyOfMatch;
            bool noneOfMatch;

            allOfMatch = (match.AllOfTypeIndicies.Count > 0) ? match.AllOfTypeIndicies.All(entity.ComponentTypeIndicies.Contains) : true;
            anyOfMatch = (match.AnyOfTypeIndicies.Count > 0) ? match.AnyOfTypeIndicies.Intersect(entity.ComponentTypeIndicies).Any() : true;
            noneOfMatch = (match.NoneOfTypeIndicies.Count > 0) ? !match.NoneOfTypeIndicies.All(entity.ComponentTypeIndicies.Contains) : true;

            return allOfMatch && anyOfMatch && noneOfMatch;
        }
    }
}
