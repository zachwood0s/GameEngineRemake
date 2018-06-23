using ECS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    public abstract class ThreadSafeReactiveSystem : ReactiveSystem
    {
        public ThreadSafeReactiveSystem(Scene scene) : base(scene)
        {
        }
        public sealed override void Execute(IEnumerable<Entity> entities)
        {
            foreach(Entity entity in entities)
            {
                lock (entity)
                {
                    Execute(entity);
                }
            }
        }
        public abstract void Execute(Entity entity);
    }
}
