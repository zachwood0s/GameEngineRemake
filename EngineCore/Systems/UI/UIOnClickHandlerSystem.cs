using ECS;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Systems.Global.ScriptManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS.Matching;
using EngineCore.Components.UI;
using ECS.Entities;
using EngineCore.Systems.Global.InputManager;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using EngineCore.Systems.Global;
using EngineCore.Systems.Scripting;

namespace EngineCore.Systems.UI
{
    public class UIOnClickHandlerSystem: GroupExecuteSystem, IInitializeSystem
    {
        private ScriptManager _scriptManager;
        private InputManager _inputManager;
        private UIOnClickLoaderSystem _scriptLoader;

        public UIOnClickHandlerSystem(Scene scene, InputManager inputManager, ScriptManager scriptManager) : base(scene)
        {
            _scriptManager = scriptManager;
            _inputManager = inputManager;
            _scriptLoader = new UIOnClickLoaderSystem(scene, scriptManager);
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(UIOnClickComponent), typeof(UITransformComponent), typeof(UIEventBoundsComponent));
        }

        public override void Execute(Entity entity)
        {
            entity.UpdateComponents<UIOnClickComponent, UITransformComponent, UIEventBoundsComponent>(
            (button, transform, bounds) =>
            {
                if (_inputManager.WasMousePressed(MouseButtons.Left))
                {
                    Rectangle shiftedBounds = bounds.Bounds;
                    shiftedBounds.Offset(transform.Position);
                    if (shiftedBounds.Contains(_inputManager.MousePosition))
                    {
                        button.ScriptAction?.Invoke(entity);
                    }
                }
            });
        }

        public void Initialize()
        {
            _scriptLoader.Initialize();
        }
    }
}
