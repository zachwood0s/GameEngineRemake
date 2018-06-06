using ECS.Components;
using ECS.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.EntityGroupManager
{
    public interface IEntityGroupManager
    {
        void EntityUpdatedComponent(Entity entity, int componentIndex, IComponent component);
        void EntityAddedComponent(Entity entity, int componentIndex, IComponent component);
        void EntityRemovedComponent(Entity entity, int componentIndex, IComponent component);

        void SubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added);
        void UnSubscribeToChanges(EntityChangedEventHandler updated, EntityChangedEventHandler removed, EntityChangedEventHandler added);
        bool IsMatch(Matcher match, Entity entity);
    }
}
