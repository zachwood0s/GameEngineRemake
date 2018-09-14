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
    public class UIEventBoundsComponent: ICopyableComponent
    {
        public Rectangle Bounds { get; set; }

        public IComponent Copy()
        {
            return new UIEventBoundsComponent
            {
                Bounds = Bounds
            };
        }
    }
}
