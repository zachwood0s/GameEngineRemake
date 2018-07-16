using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems;
using ECS.Systems.SystemPools;
using EngineCore.Components;
using EngineCore.Systems;
using EngineCore.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameCore : Game
    {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;

        //Scene _testScene;
        protected Dictionary<string, Scene> _scenes;
        protected Dictionary<string, EntityBuilder> _entityBuilders;
        protected Dictionary<string, SystemPoolBuilder> _systemPoolBuilders;
        
        
        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _scenes = new Dictionary<string, Scene>();
            _entityBuilders = new Dictionary<string, EntityBuilder>();
            _systemPoolBuilders = new Dictionary<string, SystemPoolBuilder>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ComponentPool.RegisterAllComponents();
            //ComponentPool.RegisterComponent<BasicTexture>();
            //ComponentPool.RegisterComponent<Transform2DComponent>();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadSystemPools();
            // TODO: use this.Content to load your game content here

            LoadScenes();
        }

        protected virtual void LoadSystemPools()
        {
            CreateSystemPoolBuilder("render")
                .With(_ => new ClearScreenSystem(_graphics.GraphicsDevice, Color.CornflowerBlue))
                .With(s => new BasicRenderingSystem(s, Content, _spriteBatch));

            CreateSystemPoolBuilder("update")
                .With(s => new TestMovementSystem(s))
                .WithFPS(200);
        }

        protected SystemPoolBuilder CreateSystemPoolBuilder(string name)
        {
            SystemPoolBuilder builder = new SystemPoolBuilder(name);
            _systemPoolBuilders.Add(name, builder);
            return builder;
        }

        protected virtual void LoadScenes()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //_testScene.CleanUp();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            // TODO: Add your update logic here
            //_testScene.Execute();

            base.Update(gameTime);
        }
    }
}
