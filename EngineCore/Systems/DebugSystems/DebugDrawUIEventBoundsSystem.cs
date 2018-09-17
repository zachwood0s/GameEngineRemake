using C3.XNA;
using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using EngineCore.Components.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.DebugSystems
{
    public class DebugDrawUIEventBoundsSystem : GroupExecuteSystem
    {

        public static Color RectangleDrawColor { get; set; } = Color.Black;
        private Func<bool> _getDebugState;
        private SpriteBatch _spriteBatch;

        public DebugDrawUIEventBoundsSystem(Scene scene, SpriteBatch spriteBatch, Func<bool> getDebugState) : base(scene)
        {
            _getDebugState = getDebugState;
            _spriteBatch = spriteBatch;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(UIEventBoundsComponent), typeof(UITransformComponent));
        }
        public override void Execute(Entity entity)
        {
            if (_getDebugState())
            {
                entity.UpdateComponents<UIEventBoundsComponent, UITransformComponent>((bounds, transform) =>
                {
                    Rectangle shifted = bounds.Bounds;
                    shifted.Offset(transform.Position);
                    _spriteBatch.DrawRectangle(shifted, RectangleDrawColor);
                });
            }
        }

    }
}
