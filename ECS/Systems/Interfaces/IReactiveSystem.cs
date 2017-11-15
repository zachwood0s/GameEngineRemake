using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems.Interfaces
{
    public interface IReactiveSystem
    {
        Watcher WatchList { get; }
        void Execute(IEnumerable<Entity> entities);
    }

    public static class IReactiveSystemExtension
    {
        public static void Clear(this IReactiveSystem system)
        {
            system.WatchList.ClearObservedEntities();
        }
        public static void Disable(this IReactiveSystem system)
        {
            system.WatchList.Disable();
        }
        public static void Enable(this IReactiveSystem system)
        {
            system.WatchList.Enable();
        }

        public static void Execute(this IReactiveSystem system)
        {
            if(system.WatchList.EntityCount != 0)
            {
                system.Execute(system.WatchList);
                system.Clear();
            }
        }
    }


    public abstract class ReactiveSystem: IReactiveSystem
    {
        protected Watcher _watchList;
        
        public Watcher WatchList
        {
            get
            {
                return _watchList;
            }
        }

        public ReactiveSystem(Scene scene)
        {
            _watchList = GetWatchList(scene);
        }

        protected abstract Watcher GetWatchList(Scene scene);

        public abstract void Execute(IEnumerable<Entity> entities);

    }
    //Wanna use extensions here I think. Would be neato
}


