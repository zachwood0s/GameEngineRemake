using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using ECS.Systems.Interfaces;
using EngineCore.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ECS.Matching;
using ECS.Entities;

namespace EngineCore.Systems.Rendering
{
    public class BasicRenderingSystem : GroupExecuteSystem, IInitializeSystem
    {
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;

        public BasicRenderingSystem(Scene s, ContentManager contentManager, SpriteBatch spriteBatch) : base(s)
        {
            _contentManager = contentManager;
            _spriteBatch = spriteBatch;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(BasicTextureComponent));
        }

        public override void Execute(Entity entity)
        {
            BasicTextureComponent tex = entity.GetComponent<BasicTextureComponent>();
            Transform2DComponent transform2D = entity.GetComponent<Transform2DComponent>();

            if(tex.Texture != null)
            {
                //_spriteBatch.Begin();
                _spriteBatch.Draw(tex.Texture, transform2D.Position, Color.White);
                //_spriteBatch.End();
            }
        }

        public void Initialize()
        {
            Group entitiesToLoad = Scene.GetGroup(new Matcher().Of<BasicTextureComponent>());
            foreach(Entity e in entitiesToLoad)
            {
                BasicTextureComponent notLoadedComponent = e.GetComponent<BasicTextureComponent>();
                Texture2D tex = _contentManager.Load<Texture2D>(notLoadedComponent.FileName);
                BasicTextureComponent loadedTexComponent = new BasicTextureComponent(tex);

                e.UpdateComponent(loadedTexComponent);
            }
        }
    }
}
