using ECS;
using ECS.Entities;
using ECS.Systems.Interfaces;
using EngineCore.Components.Scripting;
using EngineCore.Systems.Global;
using EngineCore.Systems.Global.ScriptManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Scripting
{
    public abstract class ScriptLoaderSystem<TScript, UAction> : IInitializeSystem where TScript : ScriptBaseComponent<UAction>
    {
        public abstract string DefaultFunctionName { get; set; }
        
        private Scene _scene;
        private ScriptManager _scriptManager;

        public ScriptLoaderSystem(Scene scene, ScriptManager scriptManager)
        {
            _scene = scene;
            _scriptManager = scriptManager;
        }
            
        public void Initialize()
        {
            var g = _scene.GetGroup(new ECS.Matching.Matcher().Of<TScript>());
            foreach(var e in g)
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
                        _scene,
                        DefaultFunctionName,
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
