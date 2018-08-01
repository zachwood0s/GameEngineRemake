using ECS.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.InputManager
{
    public class InputManager : IExecuteSystem, IInitializeSystem
    {
        private Dictionary<string, Axis> _axes;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previouseMouseState;
        
        public string InputFile { get; set; }

        public Vector2 MousePosition
        {
            get => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        }
        public InputManager()
        {
            _axes = new Dictionary<string, Axis>();
        }

        public void Initialize()
        {
            List<Axis> axes = JsonConvert.DeserializeObject<List<Axis>>(File.ReadAllText("./" + InputFile));
            foreach(Axis axis in axes)
            {
                _axes.Add(axis.Name, axis);
            }
        }
        public void Execute()
        {
            _previousKeyboardState = _currentKeyboardState;
            _previouseMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }

        public bool WasKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
        public bool WasKeyPressed(string keyName)
        {
            try
            {
                return WasKeyPressed((Keys)Enum.Parse(typeof(Keys), keyName));
            }
            catch
            {
                Debug.WriteLine($"Key '{keyName}' was not found");
                return false;
            }
        }
        public bool WasKeyReleased(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
        }
        public bool WasKeyReleased(string keyName)
        {
            try
            {
                return WasKeyReleased((Keys)Enum.Parse(typeof(Keys), keyName));
            }
            catch
            {
                Debug.WriteLine($"Key '{keyName}' was not found");
                return false;
            }
        }
        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }
        public bool IsKeyDown(string keyName)
        {
            try
            {
                return IsKeyDown((Keys)Enum.Parse(typeof(Keys), keyName));
            }
            catch
            {
                Debug.WriteLine($"Key '{keyName}' was not found");
                return false;
            }
        }
        public float GetAxis(string axisName)
        {
            if(_axes.TryGetValue(axisName, out Axis axis))
            {
                int axisVal = 0;
                if (IsKeyDown(axis.PositiveButton) || IsKeyDown(axis.AltPositiveButton)) axisVal += 1;
                if (IsKeyDown(axis.NegativeButton) || IsKeyDown(axis.AltNegativeButton)) axisVal -= 1;
                return Math.Max(-1, Math.Min(axisVal, 1));
            }
            else
            {
                Debug.WriteLine($"Axis '{axisName}' does not exist");
                return 0;
            }
        }
        public void AddAxis(string axisName, Axis axis)
        {
            _axes.Add(axisName, axis);
        }
    }

}

