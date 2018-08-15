using ECS;
using ECS.Entities;
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

namespace EngineCore.Systems.Global.ScriptManager
{
    public class ScriptManager : IInitializeSystem
    {
        public string RootDirectory { get; set; }

        private ScriptGlobals _scriptGlobals;
        /// <summary>
        /// These are the scripts after they've been loaded in from
        /// a file but not run or compiled yet
        /// </summary>
        private Dictionary<string, Script> _loadedScripts;
        private Dictionary<KeyValuePair<string, Scene>, ScriptState> _compiledScripts;

        public ScriptManager(ScriptGlobals defaultScriptGlobals)
        {
            _scriptGlobals = defaultScriptGlobals;
            _loadedScripts = new Dictionary<string, Script>();
            _compiledScripts = new Dictionary<KeyValuePair<string, Scene>, ScriptState>();
        }

        public void Initialize()
        {
            foreach (string file in Directory.EnumerateFiles("./" + RootDirectory, "*.csx", SearchOption.AllDirectories))
            {
                _LoadScriptFromFile(file);
            }
        }

        private void _LoadScriptFromFile(string file)
        {
            Script loadedScript = CSharpScript.Create(
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
                );
            string rootDirectory = Path.GetFullPath(RootDirectory);
            string fileName = Path.GetFullPath(file).Substring(rootDirectory.Length+1);
            fileName = fileName.Substring(0,fileName.IndexOf("."));

            _loadedScripts.Add(fileName, loadedScript);
        }

        public ScriptGlobals GetGlobals(Scene scene)
        {
            ScriptGlobals newGlobals = _scriptGlobals.Copy();
            newGlobals.Scene = scene;
            return newGlobals;
        }

        public object LoadScript(string scriptFile, string functionName, Scene scene)
        {
            scriptFile = scriptFile.Replace('/','\\');
            if(_loadedScripts.TryGetValue(scriptFile, out Script script))
            {
                if(!_compiledScripts.TryGetValue(new KeyValuePair<string, Scene>(scriptFile, scene), out ScriptState scriptState))
                {
                    Task<ScriptState> taskScriptState = script.RunAsync(GetGlobals(scene));
                    taskScriptState.Wait();
                    scriptState = taskScriptState.Result;
                    _compiledScripts.Add(new KeyValuePair<string, Scene>(scriptFile, scene), scriptState);
                }
                return scriptState.GetVariable(functionName).Value;
            }
            else
            {
                Debug.WriteLine($"Failed to load script! Script file {scriptFile}.csx could not be found");
            }
            return null;
        }

        public T LoadScript<T>(string scriptFile, string functionName, Scene scene) where T: class
        {
            var script = LoadScript(scriptFile, functionName, scene);
            if (script != null) return (T) script;
            return null;
        }
    }
}
