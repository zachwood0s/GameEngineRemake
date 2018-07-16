using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems;
using EngineCore.Components;
using EngineCore.Systems;
using EngineCore.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : GameCore
    {
        protected override void LoadScenes()
        {
            EntityBuilder builder = new EntityBuilder()
                .With<Transform2DComponent>()
                .With(() => new BasicTexture("test"));
            Scene scene1 = new Scene();
            scene1.AddSystemPoolFromBuilder(_systemPoolBuilders["render"]);
            scene1.AddSystemPoolFromBuilder(_systemPoolBuilders["update"]);

            for(int i = 0; i<10000; i++)
            {
                Entity e2 = scene1.CreateEntityFromBuilder(builder);
                e2.UpdateComponent((Transform2DComponent comp) => comp.Position = new Vector2((40*i) % 599, (40*i) % 397));
            }


            scene1.Initialize();
            _scenes.Add("test", scene1);

            Scene scene2 = new Scene();
            scene2.AddSystemPoolFromBuilder(_systemPoolBuilders["render"]);
            scene2.AddSystemPoolFromBuilder(_systemPoolBuilders["update"]);

            Entity e3 = scene2.CreateEntityFromBuilder(builder);


            scene2.Initialize();
            _scenes.Add("test2", scene2);
        }
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            // TODO: Add your update logic here
            _scenes["test"].Execute();
            //_testScene.Execute();

            base.Update(gameTime);
        }
    }
}
