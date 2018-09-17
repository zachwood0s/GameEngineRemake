using ECS;
using ECS.Components;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Components.Scripting;
using EngineCore.Components.UI;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Systems.Global.ScriptManager;
using EngineCore.Systems.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.UI.Events
{
    public abstract class UIEventHandlerBaseSystem<TEvent, TAction>: GroupExecuteSystem, IInitializeSystem
                                                                    where TEvent : ScriptBaseComponent<TAction>
                                                                    where TAction: class
    {
        private ScriptManager _scriptManager;
        private InputManager _inputManager;
        private ScriptLoaderSystem<TEvent, TAction> _scriptLoader;

        protected ScriptManager ScriptManager => _scriptManager;
        protected InputManager InputManager => _inputManager;

        public UIEventHandlerBaseSystem(Scene scene, InputManager inputManager, ScriptManager scriptManager): base(scene)
        {
            _scriptManager = scriptManager;
            _inputManager = inputManager;
            _scriptLoader = GetLoader();
        }

        protected abstract ScriptLoaderSystem<TEvent, TAction> GetLoader();

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(TEvent), typeof(UITransformComponent), typeof(UIEventBoundsComponent));
        }

        public void Initialize()
        {
            _scriptLoader.Initialize();
        }
    }
}
