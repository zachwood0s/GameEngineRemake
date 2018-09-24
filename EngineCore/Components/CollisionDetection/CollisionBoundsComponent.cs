using ECS;
using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.CollisionDetection
{
    [Component]
    public class CollisionBoundsComponent : ICopyableComponent
    {
        public List<CollisionGroup> CollisionGroups = new List<CollisionGroup>();
        public int ID { get; set; }

        public IComponent Copy()
        {
            return new CollisionBoundsComponent { CollisionGroups = CollisionGroups };
        }
    }
    public class CollisionGroup
    {
        public string Name { get; set; }
        public List<Polygon> Colliders { get; set; }
        public bool Colliding { get; set; }
        
    }
    public class Polygon
    {
        #region Read In

        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public List<Vector2> Points { get; set; } = new List<Vector2>();

        #endregion

        #region Non Read In

        public List<Vector2> Edges { get; set; } = new List<Vector2>();
        public List<Vector2> AbsolutePoints { get; set; } = new List<Vector2>();

        #endregion
    }
}
