using ECS;
using ECS.Components;
using ECS.Systems.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests
{
    [TestClass]
    public class SystemTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
            ComponentPool.RegisterAllComponents();
        }

        [TestMethod]
        public void SimpleWatcher()
        {
            System sys = new System();
            
        }

    }

    public class System : IReactiveSystem
    {
        public Watcher WatchList
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Execute(IEnumerable<Entity> entities)
        {
            foreach(Entity e in entities)
            {

            }
        }
    }
}
