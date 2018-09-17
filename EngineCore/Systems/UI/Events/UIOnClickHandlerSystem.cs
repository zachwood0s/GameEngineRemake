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
using EngineCore.Components.UI.Events;

namespace EngineCore.Systems.UI.Events
{
    public class UIOnClickHandlerSystem: UIEventHandlerBaseSystem<UIOnClickComponent, Action<Entity>>
    {

        public UIOnClickHandlerSystem(Scene scene, InputManager inputManager, ScriptManager scriptManager) 
            : base(scene, inputManager, scriptManager)
        {
        }
        protected override ScriptLoaderSystem<UIOnClickComponent, Action<Entity>> GetLoader() =>
            new UIOnClickLoaderSystem(Scene, ScriptManager);

        public override void Execute(Entity entity)
        {
            entity.UpdateComponents<UIOnClickComponent, UITransformComponent, UIEventBoundsComponent>(
            (button, transform, bounds) =>
            {
                Rectangle shiftedBounds = bounds.Bounds;
                shiftedBounds.Offset(transform.Position);
                if (InputManager.WasMousePressed(MouseButtons.Left))
                {
                    if (shiftedBounds.Contains(InputManager.MousePosition))
                    {
                        button.WasPressed = true;
                    }
                }
                if (button.WasPressed && InputManager.WasMouseReleased(MouseButtons.Left))
                {
                    if (shiftedBounds.Contains(InputManager.MousePosition))
                    {
                        button.ScriptAction?.Invoke(entity);
                        button.WasPressed = false;
                    }
                }
            });
        }

    }
}
