using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Components.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.UI
{
    public class UITextRenderingSystem : GroupExecuteSystem, IInitializeSystem
    {
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;

        public UITextRenderingSystem(Scene scene, ContentManager contentManager, SpriteBatch spriteBatch) : base(scene)
        {
            _contentManager = contentManager;
            _spriteBatch = spriteBatch;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(UITextComponent), typeof(UITransformComponent));
        }

        public override void Execute(Entity entity)
        {
            entity.UpdateComponents<UITransformComponent, UITextComponent>((transform, text) =>
            {
                if(text.LoadedFont != null)
                {
                    _spriteBatch.DrawString(text.LoadedFont, text.Text, transform.Position, text.TextColor);
                }
            });
        }

        public void Initialize()
        {
            Group entitiesToLoad = Scene.GetGroup(new Matcher().Of<UITextComponent>());
            foreach(Entity entity in entitiesToLoad)
            {
                entity.UpdateComponent<UITextComponent>(comp =>
                {
                    comp.LoadedFont = _contentManager.Load<SpriteFont>(comp.FontName);
                });
            }
        }
    }
}
