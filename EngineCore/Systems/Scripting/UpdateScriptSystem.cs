using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Components.Scripting;
using EngineCore.Scripting;
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
        public static string RootDirectory { get; set; }
        public static string DefaultUpdateFunctionName { get; set; }

        private ScriptGlobals _scriptGlobals;
        private Dictionary<string, ScriptState> _loadedScripts;

        public UpdateScriptSystem(Scene scene, ScriptGlobals defaultScriptGlobals) : base(scene)
        {
            _scriptGlobals = defaultScriptGlobals.Copy();
            _scriptGlobals.Scene = scene;

            _loadedScripts = new Dictionary<string, ScriptState>();
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().Of<UpdateScriptComponent>();
        }
        public override void Execute(Entity entity)
        {
            entity.UpdateComponent<UpdateScriptComponent>(scriptComponent =>
            {
                scriptComponent.UpdateFunction?.Invoke(entity);
            });
        }

        public void Initialize()
        {
            foreach (string file in Directory.EnumerateFiles("./" + RootDirectory, "*.csx", SearchOption.AllDirectories))
            {
                _LoadScriptFromFile(file);
            }
            foreach(Entity e in Group)
            {
                _LoadScriptIntoEntity(e);
            }
        }
    
        private async void _LoadScriptFromFile(string file)
        {
            ScriptState loadedScript = await CSharpScript.Create(
                File.ReadAllText(file),
                globalsType: typeof(ScriptGlobals),
                options: ScriptOptions.Default
                .WithReferences(
                    typeof(Entity).Assembly,
                    typeof(Microsoft.Xna.Framework.Vector2).Assembly,
                    typeof(UpdateScriptComponent).Assembly
                    )
                .WithImports(
                    "ECS",
                    "ECS.Entities", 
                    "ECS.Components",
                    "EngineCore.Components",
                    "Microsoft.Xna.Framework"
                    )
                ).RunAsync(_scriptGlobals);
            _loadedScripts.Add(Path.GetFileNameWithoutExtension(file), loadedScript);
        }

        private void _LoadScriptIntoEntity(Entity entity)
        {
            entity.UpdateComponent<UpdateScriptComponent>(updateScriptComponent =>
            {
                if (_loadedScripts.TryGetValue(updateScriptComponent.ScriptFile, out ScriptState scriptState))
                {
                    if(updateScriptComponent.FunctionName != null)
                    {
                        updateScriptComponent.UpdateFunction = (Action<Entity>) scriptState.GetVariable(updateScriptComponent.FunctionName).Value;
                    }
                    else if(DefaultUpdateFunctionName != null)
                    {
                        updateScriptComponent.UpdateFunction = (Action<Entity>) scriptState.GetVariable(DefaultUpdateFunctionName).Value;
                    }
                    else
                    {
                        Debug.WriteLine("UpdateScriptSystem: No default update function name set! No update scripts will be loaded!");
                        return;
                    }
                }
                else
                {
                    Debug.WriteLine($"Failed to load script for entity! Script file {updateScriptComponent.ScriptFile}.csx could not be found");
                }
            });
        }
    }
}
