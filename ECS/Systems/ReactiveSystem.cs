using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    public abstract class ReactiveSystem: IReactiveSystem
    {
        protected Watcher _watchList;
        protected Matcher _matcher;
        public Watcher WatchList => _watchList;

        public ReactiveSystem(Scene scene)
        {
            _matcher = GetMatcher();
            _watchList = new Watcher(scene.GetGroup(_matcher));
        }

        protected abstract Matcher GetMatcher();

        public abstract void Execute(IEnumerable<Entity> entities);
        
        public void Execute()
        {
            if(WatchList.EntityCount != 0)
            {
                Execute(WatchList);
                this.Clear();
            }
        }
    }
}
