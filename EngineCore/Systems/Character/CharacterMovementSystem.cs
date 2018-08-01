using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
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
    public class CharacterMovementSystem : IExecuteSystem
    {
        private Scene _scene;
        private Group _group;
        private InputManager _inputManager;

        public CharacterMovementSystem(Scene s, InputManager inputManager) 
        {
            _scene = s;
            _group = _scene.GetGroup(GetMatcher());
            _inputManager = inputManager;
        }
        public Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(CharacterMovementComponent));
        }

        public void Execute()
        {
            _group.UpdateAllEntitiesInGroup(
                (Entity entity, Transform2DComponent transform) =>
                {
                    var movement = entity.GetComponent<CharacterMovementComponent>();
                    Vector2 moveVector = new Vector2(
                         movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.HorizontalInputAxis),
                         movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.VerticalInputAxis)
                        );
                    transform.Position = Vector2.Add(transform.Position, moveVector);

                    if(transform.Position.X > 600)
                    {
                        if (entity.HasComponent<BasicTextureComponent>())
                        {
                            entity.Remove<BasicTextureComponent>();
                        }
                    }
                });
            /*
            _group.ApplyToAllEntities(entity =>
            {
                var transform = entity.GetComponent<Transform2DComponent>();
                var movement = entity.GetComponent<CharacterMovementComponent>();

                Vector2 moveVector = new Vector2(
                     movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.HorizontalInputAxis),
                     movement.CharacterMovementSpeed * _inputManager.GetAxis(movement.VerticalInputAxis)
                    );
                transform.Position = Vector2.Add(transform.Position, moveVector);

                entity.UpdateComponent(transform);
                if (transform.Position.X > 600)
                {
                    //_scene.RemoveEntity(entity);
                    if (entity.HasComponent<BasicTextureComponent>())
                        entity.Remove<BasicTextureComponent>();
                }
            });
            */
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
