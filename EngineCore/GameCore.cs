using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems;
using ECS.Systems.Interfaces;
using ECS.Systems.SystemPools;
using EngineCore.Components;
using EngineCore.Scripting;
using EngineCore.Systems;
using EngineCore.Systems.Character;
using EngineCore.Systems.Global.Animation;
using EngineCore.Systems.Global.EntityBuilderLoader;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Systems.Global.SceneLoader;
using EngineCore.Systems.Global.SceneManager;
using EngineCore.Systems.Global.ScriptManager;
using EngineCore.Systems.Global.SettingsLoader;
using EngineCore.Systems.Rendering;
using EngineCore.Systems.Scripting;
using EngineCore.Systems.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ScriptGlobals _defaultScriptGlobals;
        protected SystemPool GlobalSystems => _globalSystems;
        protected SpriteBatch SpriteBatch => _spriteBatch;
        protected GraphicsDeviceManager Graphics => _graphics;

        //Scene _testScene;
        private Dictionary<string, Scene> _scenes;
        private Dictionary<string, EntityBuilder> _entityBuilders;
        private Dictionary<string, SystemPoolBuilder> _systemPoolBuilders;
        private IInitializeSystem _settingsLoader; 
        protected Dictionary<string, Scene> Scenes => _scenes;
        protected Dictionary<string, EntityBuilder> EntityBuilders => _entityBuilders;
        protected Dictionary<string, SystemPoolBuilder> SystemPoolBuilders => _systemPoolBuilders;
        protected IInitializeSystem SettingsLoader
        {
            get => _settingsLoader;
            set => _settingsLoader = value;
        }
        
        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _scenes = new Dictionary<string, Scene>();
            _entityBuilders = new Dictionary<string, EntityBuilder>();
            _systemPoolBuilders = new Dictionary<string, SystemPoolBuilder>();
            _globalSystems = new SystemPool("Global");

            //IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
            //_graphics.SynchronizeWithVerticalRetrace = false;
            _settingsLoader = new SettingsLoader(this, _graphics, Content);
            ((SettingsLoader) _settingsLoader).RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            _settingsLoader.Initialize();
            // TODO: Add your initialization logic here
            ComponentPool.RegisterAllComponents();
            //ComponentPool.RegisterComponent<BasicTexture>();
            //ComponentPool.RegisterComponent<Transform2DComponent>();
            Graphics.ApplyChanges();
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
            #region Global Systems
            EntityBuilderLoader builderLoader = new EntityBuilderLoader(_entityBuilders);
            builderLoader.RootDirectory = "Content/EntityBuilders";
            _globalSystems.Register(builderLoader);

            SceneLoader sceneLoader = new SceneLoader(_scenes, _systemPoolBuilders, _entityBuilders);
            sceneLoader.RootDirectory = "Content/Scenes/ZachScenes";
            _globalSystems.Register(sceneLoader);

            SceneManager sceneManager = new SceneManager(_scenes);
            _globalSystems.Register(sceneManager);

            InputManager inputManager = new InputManager();
            inputManager.InputFile = "Content/keybindings.json";
            _globalSystems.Register(inputManager);
            /*
            GlobalAnimationSystem globalAnimationSystem = new GlobalAnimationSystem();
            globalAnimationSystem.InputFile = "Content/animations.json";
            _globalSystems.Register(globalAnimationSystem);
            */

            _defaultScriptGlobals = new ScriptGlobals()
            {
                InputManager = inputManager,
                SceneManager = sceneManager
            };

            ScriptManager scriptManager = new ScriptManager(_defaultScriptGlobals);
            scriptManager.RootDirectory = "Content/Scripts";
            _globalSystems.Register(scriptManager);

            #endregion

            #region System Pools

            CreateSystemPoolBuilder("Render")
                .With(_ => new ClearScreenSystem(_graphics.GraphicsDevice, Color.CornflowerBlue))
                .With(_ => new SpriteBatchBeginSystem(_spriteBatch))
                .With(s => new BasicRenderingSystem(s, Content, _spriteBatch))
                .With(s => new UITextRenderingSystem(s, Content, _spriteBatch))
                //.With(s => new AnimationSystem(s, _spriteBatch, Content, inputManager, globalAnimationSystem))
                .With(_ => new SpriteBatchEndSystem(_spriteBatch));

            CreateSystemPoolBuilder("Update")
                .With(s => new CharacterMovementSystem(s, inputManager))
                .With(s => new UpdateScriptSystem(s, scriptManager))
                .WithFPS(200);

            CreateSystemPoolBuilder("UIEvents")
                .With(s => new UIOnClickHandlerSystem(s, inputManager, scriptManager))
                .WithFPS(60);

            #endregion
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

            /*            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        {
                            Debug.WriteLine(_scenes["Scene2"].GetSystemPoolByName("Update").CurrentFps);
                            Debug.WriteLine(_scenes["Scene2"].GetSystemPoolByName("Render").CurrentFps);
                        }*/
            base.Update(gameTime); }
    }
}
