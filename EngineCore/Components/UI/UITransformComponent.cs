using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.UI
{
    [Component]
    public class UITransformComponent : IComponentHasDefault, ICopyableComponent
    {
        public Vector2 Position { get; set; }

        public void SetDefaults()
        {
            Position = new Vector2(0, 0);
        }
        public IComponent Copy()
        {
            return new UITransformComponent() { Position = Position };
        }
    }
}
