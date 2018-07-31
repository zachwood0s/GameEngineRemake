using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using EngineCore.Components;
using EngineCore.Systems.Global.InputManager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Character
{
    public class CharacterMovementSystem : GroupExecuteSystem
    {
        private InputManager _inputManager;

        public CharacterMovementSystem(Scene s, InputManager inputManager) : base(s)
        {
            _inputManager = inputManager;
        }
        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(CharacterMovementComponent));
        }

        public override void Execute(Entity entity)
        {
            var transform = entity.GetComponent<Transform2DComponent>();
            var movement = entity.GetComponent<CharacterMovementComponent>();

            Vector2 moveVector = new Vector2(
                 movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.HorizontalInputAxis),
                 movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.VerticalInputAxis)
                );
            transform.Position = Vector2.Add(transform.Position, moveVector);

            entity.UpdateComponent(transform);
            /*
            entity.UpdateComponents<Transform2DComponent, CharacterMovementComponent>(
                (transform, movement) =>
                {
                    Vector2 moveVector = new Vector2(
                         movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.HorizontalInputAxis),
                         movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.VerticalInputAxis)
                        );
                    transform.Position = Vector2.Add(transform.Position, moveVector);
                });
                */
                
        }

    }
}
