using ECS.Systems.Interfaces;
using ExampleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace EngineCore.Systems.Global.SettingsLoader
{
    public class SettingsLoader : IInitializeSystem
    {
        protected GameCore _gameCore;
        protected GraphicsDeviceManager _graphics;
        protected ContentManager _content;
        protected Action<string> _sceneSetFunction;

        public string RootDirectory { get; set; }
        public string FileName { get; set; } = "settings.json";

        public SettingsLoader(GameCore core, GraphicsDeviceManager graphics, ContentManager content, Action<string> sceneSetFunction)
        {
            _gameCore = core;
            _graphics = graphics;
            _content = content;
            _sceneSetFunction = sceneSetFunction;
        }

        public virtual void Initialize()
        {
            SettingsConstruct settings = new SettingsConstruct();
            try
            {
                settings = JsonConvert.DeserializeObject<SettingsConstruct>(File.ReadAllText("./" + RootDirectory + "/" + FileName));
            }
            catch(Exception e)
            {
                Debug.WriteLine("Failed to load settings file, using defaults instead");
                Debug.WriteLine($"Message given by exception {e.Message}");
            }
            _LoadSettings(settings);
        }

        protected void _LoadSettings(SettingsConstruct construct)
        {
            _gameCore.IsFixedTimeStep = construct.LockedRenderFps;
            _gameCore.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / construct.TargetRenderFps);
            _gameCore.IsMouseVisible = construct.IsMouseVisible;

            _content.RootDirectory = construct.ContentDirectory;

            _graphics.IsFullScreen = construct.FullScreen;
            _graphics.PreferredBackBufferWidth = construct.WindowWidth;
            _graphics.PreferredBackBufferHeight = construct.WindowHeight;
            _graphics.SynchronizeWithVerticalRetrace = construct.VSync;

            _graphics.ApplyChanges();

            _sceneSetFunction(construct.StartingScene);
        }
    }

    public class SettingsConstruct
    {
        public string ContentDirectory { get; set; } = "Content";
        public bool LockedRenderFps { get; set; } = true;
        public int TargetRenderFps { get; set; } = 60;
        public bool VSync { get; set; } = true;
        public bool IsMouseVisible { get; set; } = true;
        public int WindowWidth { get; set; } = 600;
        public int WindowHeight { get; set; } = 400;
        public bool FullScreen { get; set; } = false;
        public string StartingScene { get; set; } = "";
    }
}
