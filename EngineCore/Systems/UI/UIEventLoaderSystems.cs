using ECS;
using ECS.Entities;
using EngineCore.Components.UI;
using EngineCore.Systems.Global.ScriptManager;
using EngineCore.Systems.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.UI
{
    public class UIOnClickLoaderSystem : ScriptLoaderSystem<UIOnClickComponent, Action<Entity>>
    {
        public UIOnClickLoaderSystem(Scene scene, ScriptManager scriptManager) : base(scene, scriptManager)
        {
        }
        public override string DefaultFunctionName { get; set; } = "OnClick"; 
    }
}
