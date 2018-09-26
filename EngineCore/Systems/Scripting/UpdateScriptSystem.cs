using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Components.Scripting;
using EngineCore.Scripting;
using EngineCore.Systems.Global;
using EngineCore.Systems.Global.ScriptManager;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Scripting
{
    public class UpdateScriptSystem : GroupExecuteSystem, IInitializeSystem
    {
        private ScriptManager _scriptManager;
        private UpdateScriptLoaderSystem _scriptLoader;

        public UpdateScriptSystem(Scene scene, ScriptManager scriptManager) : base(scene)
        {
            _scriptManager = scriptManager;
            _scriptLoader = new UpdateScriptLoaderSystem(Scene, _scriptManager);
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().Of<UpdateScriptComponent>();
        }
        public override void Execute(Entity entity)
        {
            var scriptAction = entity.GetComponent<UpdateScriptComponent>()?.ScriptAction;
            scriptAction?.Invoke(entity);
        }

        public void Initialize()
        {
            _scriptLoader.Initialize();
        }
    }
}
