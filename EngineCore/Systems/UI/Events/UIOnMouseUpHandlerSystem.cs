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
using EngineCore.Systems.UI.Events;

namespace EngineCore.Systems.UI
{
    public class UIOnMouseUpHandlerSystem: UIEventHandlerBaseSystem<UIOnMouseUpComponent, Action<Entity>>
    {
        public UIOnMouseUpHandlerSystem(Scene scene, InputManager inputManager, ScriptManager scriptManager) 
            : base(scene, inputManager, scriptManager)
        {
        }
        protected override ScriptLoaderSystem<UIOnMouseUpComponent, Action<Entity>> GetLoader() => 
            new UIOnMouseUpLoaderSystem(Scene, ScriptManager); 

        public override void Execute(Entity entity)
        {
            Action<Entity> scriptAction = null;
            entity.GetComponents<UIOnMouseUpComponent, UITransformComponent, UIEventBoundsComponent>(
            (button, transform, bounds) =>
            {
                if (InputManager.WasMouseReleased(MouseButtons.Left))
                {
                    Rectangle shiftedBounds = bounds.Bounds;
                    shiftedBounds.Offset(transform.Position);
                    if (shiftedBounds.Contains(InputManager.MousePosition))
                    {
                        scriptAction = button.ScriptAction;
                    }
                }
            });
            scriptAction?.Invoke(entity);
        }
    }
}
