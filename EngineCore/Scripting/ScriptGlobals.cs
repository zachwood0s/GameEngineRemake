using ECS;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Systems.Global.SceneManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Scripting
{
    public class ScriptGlobals
    {
        public InputManager InputManager { get; set; }
        public SceneManager SceneManager { get; set; }
        public Scene Scene { get; set; }
        public ScriptGlobals Copy()
        {
            return new ScriptGlobals()
            {
                InputManager = InputManager,
                SceneManager = SceneManager
            };
        }
    }
}
