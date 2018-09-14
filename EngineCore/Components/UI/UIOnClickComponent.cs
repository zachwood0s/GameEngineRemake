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

namespace EngineCore.Components.UI
{
    [Component]
    public class UIOnClickComponent : ScriptBaseComponent<Action<Entity>>
    {
        protected override ScriptBaseComponent<Action<Entity>> CopyInstantiator()
        {
            return new UIOnClickComponent();
        }
    }
}
