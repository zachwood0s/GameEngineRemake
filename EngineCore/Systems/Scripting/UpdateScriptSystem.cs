﻿using ECS;
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
        public static string DefaultUpdateFunctionName { get; set; } = "Update";

        private ScriptManager _scriptManager;

        public UpdateScriptSystem(Scene scene, ScriptManager scriptManager) : base(scene)
        {
            _scriptManager = scriptManager;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().Of<UpdateScriptComponent>();
        }
        public override void Execute(Entity entity)
        {
            entity.UpdateComponent<UpdateScriptComponent>(scriptComponent =>
            {
                scriptComponent.ScriptAction?.Invoke(entity);
            });
        }

        public void Initialize()
        {
            foreach(Entity e in Group)
            {
                _LoadScriptIntoEntity(e);
            }
        }
    
        private void _LoadScriptIntoEntity(Entity entity)
        {
            entity.UpdateComponent<UpdateScriptComponent>(updateScriptComponent =>
            {
                try
                {
                    updateScriptComponent.ScriptAction = LoaderHelper.GetScriptActionFromComponent(
                        updateScriptComponent,
                        Scene,
                        DefaultUpdateFunctionName,
                        _scriptManager
                        );
                }
                catch(ArgumentNullException ex)
                {
                    Debug.WriteLine("UpdateScriptSystem: {0}", ex.Message);
                }
            });
        }
    }
}
