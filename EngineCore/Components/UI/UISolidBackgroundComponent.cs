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
    public class UISolidBackgroundComponent : ICopyableComponent
    {
        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }
        public IComponent Copy()
        {
            return new UISolidBackgroundComponent()
            {
                Bounds = Bounds,
                Color = Color
            };
        }
    }
}
