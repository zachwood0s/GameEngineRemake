using ECS.Attributes;
using ECS.Components;
using ECS.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.UI
{
    [Component]
    public class UIButtonComponent: ICopyableComponent
    {
        public Action<Entity> ButtonAction { get; set; }
        public Rectangle ButtonBounds { get; set; }

        public IComponent Copy()
        {
            return new UIButtonComponent()
            {
                ButtonAction = ButtonAction,
                ButtonBounds = ButtonBounds
            };
        }
    }
}
