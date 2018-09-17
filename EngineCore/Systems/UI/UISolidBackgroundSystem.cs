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

namespace EngineCore.Systems.UI
{
    public class UISolidBackgroundSystem : GroupExecuteSystem
    {
        private SpriteBatch _spriteBatch;

        public UISolidBackgroundSystem(Scene scene, SpriteBatch spriteBatch) : base(scene)
        {
            _spriteBatch = spriteBatch;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(UITransformComponent), typeof(UISolidBackgroundComponent));
        }
        public override void Execute(Entity entity)
        {
            entity.UpdateComponents<UITransformComponent, UISolidBackgroundComponent>((transform, background) =>
            {
                Rectangle shifted = background.Bounds;
                shifted.Offset(transform.Position);
                _spriteBatch.FillRectangle(shifted, background.Color);
            });
        }
    }
}
