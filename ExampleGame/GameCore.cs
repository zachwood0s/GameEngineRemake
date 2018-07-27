using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems;
using EngineCore.Components;
using EngineCore.Systems;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Systems.Global.SceneManager;
using EngineCore.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : GameCore
    {

        protected override void LoadSystemPools()
        {
            base.LoadSystemPools();

            InputManager inputManager = GlobalSystems.GetSystem<InputManager>();
            CreateSystemPoolBuilder("time").With((scene) =>
            {
                return new TimeSlow(inputManager, scene.GetSystemPoolByName("Update") as ThreadedSystemPool);
            }).WithFPS(10);
        }

        protected override void LoadScenes()
        {
            SceneManager sceneManager = GlobalSystems.GetSystem<SceneManager>();
            sceneManager.ChangeScene("Scene2");

            InputManager inputManager = GlobalSystems.GetSystem<InputManager>();
            inputManager.AddAxis("horizontal", new Axis(Keys.Right, Keys.D, Keys.Left, Keys.A));
            inputManager.AddAxis("vertical", new Axis(Keys.Down, Keys.S, Keys.Up, Keys.W));
            inputManager.AddAxis("time", new Axis(Keys.Space, Keys.Space, Keys.J, Keys.J));

            var fuckyscene = Scenes["Scene2"];
            var random = new Random();

            for(int i = 0; i< 2000; i++)
            {
                var entity = EntityBuilders["TestBuilder"].Build(fuckyscene);
                entity.UpdateComponent<Transform2DComponent>(comp => {
                    comp.Position = new Vector2(random.Next(6000), random.Next(4000));
                    comp.Rotation = (float) random.NextDouble() * 2 ;
                    });
            }

            base.LoadScenes();
        }
    }
}
