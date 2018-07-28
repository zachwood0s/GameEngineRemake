using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems;
using ECS.Systems.SystemPools;
using EngineCore.Components;
using EngineCore.Systems;
using EngineCore.Systems.Character;
using EngineCore.Systems.Global.EntityBuilderLoader;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Systems.Global.SceneLoader;
using EngineCore.Systems.Global.SceneManager;
using EngineCore.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SystemPool _globalSystems;
        protected SystemPool GlobalSystems => _globalSystems;
        protected SpriteBatch SpriteBatch => _spriteBatch;
        protected GraphicsDeviceManager Graphics => _graphics;

        //Scene _testScene;
        private Dictionary<string, Scene> _scenes;
        private Dictionary<string, EntityBuilder> _entityBuilders;
        private Dictionary<string, SystemPoolBuilder> _systemPoolBuilders;
        protected Dictionary<string, Scene> Scenes => _scenes;
        protected Dictionary<string, EntityBuilder> EntityBuilders => _entityBuilders;
        protected Dictionary<string, SystemPoolBuilder> SystemPoolBuilders => _systemPoolBuilders;
        
        
        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _scenes = new Dictionary<string, Scene>();
            _entityBuilders = new Dictionary<string, EntityBuilder>();
            _systemPoolBuilders = new Dictionary<string, SystemPoolBuilder>();
            _globalSystems = new SystemPool("Global");
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
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
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
            _globalSystems.Initialize();
            // TODO: use this.Content to load your game content here

            LoadScenes();

        }

        protected virtual void LoadSystemPools()
        {
            EntityBuilderLoader builderLoader = new EntityBuilderLoader(_entityBuilders);
            builderLoader.RootDirectory = "Content/EntityBuilders";
            _globalSystems.Register(builderLoader);

            SceneLoader sceneLoader = new SceneLoader(_scenes, _systemPoolBuilders, _entityBuilders);
            sceneLoader.RootDirectory = "Content/Scenes";
            _globalSystems.Register(sceneLoader);

            SceneManager sceneManager = new SceneManager(_scenes);
            _globalSystems.Register(sceneManager);

            InputManager inputManager = new InputManager();
            inputManager.InputFile = "Content/keybindings.json";
            _globalSystems.Register(inputManager);

            CreateSystemPoolBuilder("Render")
                .With(_ => new ClearScreenSystem(_graphics.GraphicsDevice, Color.CornflowerBlue))
                .With(_ => new SpriteBatchBeginSystem(_spriteBatch))
                .With(s => new BasicRenderingSystem(s, Content, _spriteBatch))
                .With(_ => new SpriteBatchEndSystem(_spriteBatch));

            CreateSystemPoolBuilder("Update")
                .With(s => new CharacterMovementSystem(s, inputManager))
                .WithFPS(300);
        }

        protected SystemPoolBuilder CreateSystemPoolBuilder(string name)
        {
            SystemPoolBuilder builder = new SystemPoolBuilder(name);
            _systemPoolBuilders.Add(name, builder);
            return builder;
        }

        protected virtual void LoadScenes()
        {
            foreach(Scene scene in _scenes.Values)
            {
                scene.Initialize();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            foreach(Scene scene in _scenes.Values)
            {
                scene.CleanUp();
            }
            _globalSystems.CleanUp();
            //_testScene.CleanUp();
        }

        protected override void Update(GameTime gameTime)
        {
            _globalSystems.Execute();

            base.Update(gameTime);
        }
    }
}
