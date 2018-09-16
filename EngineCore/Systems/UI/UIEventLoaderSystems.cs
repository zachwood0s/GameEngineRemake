using ECS;
using ECS.Entities;
using EngineCore.Components.UI;
using EngineCore.Components.UI.Events;
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
    public class UIOnMouseEnterLoaderSystem : ScriptLoaderSystem<UIOnMouseEnterComponent, Action<Entity>>
    {
        public UIOnMouseEnterLoaderSystem(Scene scene, ScriptManager scriptManager) : base(scene, scriptManager)
        {
        }
        public override string DefaultFunctionName { get; set; } = "OnMouseEnter"; 
    }
    public class UIOnMouseExitLoaderSystem : ScriptLoaderSystem<UIOnMouseExitComponent, Action<Entity>>
    {
        public UIOnMouseExitLoaderSystem(Scene scene, ScriptManager scriptManager) : base(scene, scriptManager)
        {
        }
        public override string DefaultFunctionName { get; set; } = "OnMouseExit"; 
    }
}
