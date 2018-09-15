using ECS;
using ECS.Entities;
using EngineCore.Components.Scripting;
using EngineCore.Systems.Global.ScriptManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Scripting
{
    public class UpdateScriptLoaderSystem : ScriptLoaderSystem<UpdateScriptComponent, Action<Entity>>
    {
        public UpdateScriptLoaderSystem(Scene scene, ScriptManager scriptManager) : base(scene, scriptManager)
        {
        }

        public override string DefaultFunctionName { get; set; } = "Update"; 
    }
}
