using ECS;
using ECS.Attributes;
using ECS.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests
{
    [TestClass]
    public class ComponentPoolTests
    {
        private Scene _scene;

        [TestInitialize]
        public void Init()
        {
            _scene = new Scene();
        }

        [TestMethod]
        public void RegisterAllComponents()
        {
            ComponentPool.RegisterAllComponents();

            Assert.AreNotEqual(-1, ComponentPool.GetComponentIndex<Position>());
        }

        [Component]
        public class Position : IComponentHasDefault
        {
            public int X { get; set; }
            public int Y { get; set; }
            public void SetDefaults()
            {
                X = 10;
                Y = 10;
            }
        }
    }
}
