using ECS.Attributes;
using ECS.Components;
using ECS.Entities;
using EngineCore.Components.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.UI.Events
{
    [Component]
    public class UIOnMouseDownComponent : ScriptBaseComponent<Action<Entity>>
    {
        public bool WasTriggered { get; set; }
        protected override ScriptBaseComponent<Action<Entity>> CopyInstantiator()
        {
            return new UIOnMouseDownComponent();
        }
        public override IComponent Copy()
        {
            var clone = base.Copy() as UIOnMouseDownComponent;
            clone.WasTriggered = WasTriggered;
            return clone;
        }
    }
}
