using ECS.Attributes;
using ECS.Components;
using ECS.Entities;
using EngineCore.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.Scripting
{
    [Component]
    public class UpdateScriptComponent : ScriptBaseComponent<Action<Entity>>
    {
        protected override ScriptBaseComponent<Action<Entity>> CopyInstantiator()
        {
            return new UpdateScriptComponent();
        }
    }
}
