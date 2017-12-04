using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    public abstract class GroupExecuteSystem : IExecuteSystem
    {
        private Scene _scene;
        private Group _group;

        protected Scene Scene => _scene;

        public GroupExecuteSystem(Scene scene)
        {
            _scene = scene;
            _group = _scene.GetGroup(GetMatcher());
        }
        public abstract Matcher GetMatcher();
        public void Execute()
        {
            foreach(Entity e in _group)
            {
                lock (e)
                {
                    Execute(e);
                }
            }
        }
        public abstract void Execute(Entity entity);
    }
}
