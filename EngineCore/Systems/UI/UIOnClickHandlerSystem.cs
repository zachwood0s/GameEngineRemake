﻿using ECS;
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

namespace EngineCore.Systems.UI
{
    public class UIOnClickHandlerSystem: GroupExecuteSystem, IInitializeSystem
    {
        public static string DefaultButtonFunctionName { get; set; } = "OnClick";
        private ScriptManager _scriptManager;
        private InputManager _inputManager;

        public UIOnClickHandlerSystem(Scene scene, InputManager inputManager, ScriptManager scriptManager) : base(scene)
        {
            _scriptManager = scriptManager;
            _inputManager = inputManager;
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
                if (_inputManager.IsMousePressed(MouseButtons.Left))
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
            foreach(Entity e in Group)
            {
                _LoadScriptIntoEntity(e);
            }
        }

        private void _LoadScriptIntoEntity(Entity e)
        {
            e.UpdateComponent<UIOnClickComponent>(button =>
            {
                try
                {
                    button.ScriptAction = LoaderHelper.GetScriptActionFromComponent(
                        button,
                        Scene,
                        DefaultButtonFunctionName,
                        _scriptManager
                        );
                }
                catch(ArgumentNullException ex)
                {
                    Debug.WriteLine("UIOnClickHandlerSystem: {0}", ex.Message);
                }
            });
        }
    }
}
