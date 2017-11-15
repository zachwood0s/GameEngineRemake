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
            System sys = new System(_scene);
            
        }

    }

    public class System : ReactiveSystem
    {
        public System(Scene scene) : base(scene)
        {
        }

        protected override Watcher GetWatchList(Scene scene)
        {
            return new Watcher(scene.GetGroup(new Matcher().Of<TestPositionComponent>()));
        }
        public override void Execute(IEnumerable<Entity> entities)
        {
            foreach(Entity e in entities)
            {

            }
        }
    }
}
