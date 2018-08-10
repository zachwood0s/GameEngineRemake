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
    public class UpdateScriptComponent : ICopyableComponent
    {
        public string ScriptFile { get; set; }
        public string FunctionName { get; set; }
        public Action<Entity> UpdateFunction { get; set; }

        public IComponent Copy()
        {
            return new UpdateScriptComponent()
            {
                ScriptFile = ScriptFile,
                FunctionName = FunctionName,
                UpdateFunction = UpdateFunction
            };
        }
    }
}
