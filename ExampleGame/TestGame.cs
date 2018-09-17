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

        }

        protected override void LoadScenes()
        {
            SceneManager sceneManager = GlobalSystems.GetSystem<SceneManager>();
            sceneManager.ChangeScene("ZachScene");
            base.LoadScenes();
        }
    }
}
