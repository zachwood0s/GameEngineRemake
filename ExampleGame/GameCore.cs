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

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();


            // TODO: Add your update logic here
            _scenes["TestScene"].Execute();
            //_testScene.Execute();

            base.Update(gameTime);
        }
    }
}
