using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using EngineCore.Components;
using ECS.Matching;
using ECS.Entities;
using EngineCore.Systems.Global.InputManager;

namespace EngineCore.Systems
{
    public class TestMovementSystem : ThreadSafeReactiveSystem
    {
        private InputManager _inputManager;
        public TestMovementSystem(Scene s, InputManager inputManager): base(s)
        {
            _inputManager = inputManager;
        }
        public override void Execute(Entity entity)
        {
            entity.UpdateComponent((Transform2DComponent comp) =>
            {
                float xInput = _inputManager.GetAxis("horizontal");
                float yInput = _inputManager.GetAxis("vertical");
                comp.Position = new Microsoft.Xna.Framework.Vector2(
                    comp.Position.X + comp.Rotation* xInput , 
                    comp.Position.Y + comp.Rotation * yInput 
                    );
            });

            //Transform2DComponent component = entity.GetComponent<Transform2DComponent>();
            //entity.UpdateComponent(new Transform2DComponent((int)component.Position.X + 1, (int)component.Position.Y));
        }

        protected override Matcher GetMatcher()
        {
            return new Matcher().Of<Transform2DComponent>();
        }
    }
} 