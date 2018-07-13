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
    public class GameCore : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Scene _testScene;
        
        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _testScene = new Scene();
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
            ComponentPool.RegisterComponent<BasicTexture>();
            ComponentPool.RegisterComponent<Transform2DComponent>();
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

            // TODO: use this.Content to load your game content here
            SystemPool rendering = new SystemPool("render");
            rendering.Register(new ClearScreenSystem(_graphics.GraphicsDevice, Color.CornflowerBlue));
            rendering.Register(new BasicRenderingSystem(_testScene, Content, _spriteBatch));

            SystemPool update = new ThreadedSystemPool("update", 200);
            update.Register(new TestMovementSystem(_testScene));

            _testScene.AddSystemPool(rendering);
            _testScene.AddSystemPool(update);

            Entity e = _testScene.CreateEntity()
                .With<Transform2DComponent>()
                .With(new BasicTexture("test"));

            Entity e2 = _testScene.CreateEntity()
                .With(new Transform2DComponent(20, 100))
                .With(new BasicTexture("test"));
            

            _testScene.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            _testScene.CleanUp();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _testScene.Execute();

            base.Update(gameTime);
        }
    }
}
