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
        private MouseState _previousMouseState;
        private GamePadState[] _currentGamePadStates;
        private GamePadState[] _previousGamePadStates;

        public string InputFile { get; set; }

        public Vector2 MousePosition
        {
            get => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        }
        public InputManager()
        {
            _axes = new Dictionary<string, Axis>();
            _currentGamePadStates = new GamePadState[4];
            _previousGamePadStates = new GamePadState[4];
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
            // Set previous states
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            for(int i = 0; i < _previousGamePadStates.Length; i++)
            {
                _previousGamePadStates[i] = _currentGamePadStates[i];
            }

            // Set current states
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
            for(int i = 0; i < _currentGamePadStates.Length; i++)
            {
                GamePadCapabilities capabilities = GamePad.GetCapabilities(i);
                if (capabilities.IsConnected) _currentGamePadStates[i] = GamePad.GetState(i);
            }

            // Temperary Testing
            float x = 0;
            if (WasGamePadPressed("A")) Console.WriteLine("A Was Pressed");
            if (WasGamePadReleased("A")) Console.WriteLine("A Was Released");
            if (IsGamePadButtonPressed("A")) Console.WriteLine("A Is Pressed");
            //Console.WriteLine("Left Thumbstick X: " + GetJoyStickAxis("Left X", raw: true));

            if (WasMousePressed("left mouse")) Console.WriteLine("Mouse Left Pressed");
            if (WasMouseReleased("left mouse")) Console.WriteLine("Mouse Left Released");
            if (WasMousePressed("right mouse")) Console.WriteLine("Mouse Right Pressed");
            if (WasMouseReleased("right mouse")) Console.WriteLine("Mouse Right Released");
            if (WasMousePressed("middle mouse")) Console.WriteLine("Mouse Middle Pressed");
            if (WasMouseReleased("middle mouse")) Console.WriteLine("Mouse Middle Released");

            //Console.WriteLine("Axis: " + GetAxis("horizontal", raw: true));
        }

        # region Key Events

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

        #endregion

        # region Mouse Events

        public bool WasMousePressed(string mouseButton)
        {
            if (mouseButton == "left mouse") return _currentMouseState.LeftButton == ButtonState.Pressed &&
                                                    _previousMouseState.LeftButton == ButtonState.Released;
            if (mouseButton == "right mouse") return _currentMouseState.RightButton == ButtonState.Pressed &&
                                                    _previousMouseState.RightButton == ButtonState.Released;
            if (mouseButton == "middle mouse") return _currentMouseState.MiddleButton == ButtonState.Pressed &&
                                                    _previousMouseState.MiddleButton == ButtonState.Released;
            else
            {
                Debug.WriteLine($"Mouse '{mouseButton}' does not exist");
                return false;
            }
        }
        public bool WasMouseReleased(string mouseButton)
        {
            if (mouseButton == "left mouse") return _currentMouseState.LeftButton == ButtonState.Released && 
                                                    _previousMouseState.LeftButton == ButtonState.Pressed;
            if (mouseButton == "right mouse") return _currentMouseState.RightButton == ButtonState.Released &&
                                                    _previousMouseState.RightButton == ButtonState.Pressed;
            if (mouseButton == "middle mouse") return _currentMouseState.MiddleButton == ButtonState.Released &&
                                                    _previousMouseState.MiddleButton == ButtonState.Pressed;
            else
            {
                Debug.WriteLine($"Mouse '{mouseButton}' does not exist");
                return false;
            }
        }
        public bool IsMousePressed(string mouseButton)
        {
            if (mouseButton == "left mouse") return _currentMouseState.LeftButton == ButtonState.Pressed;
            if (mouseButton == "right mouse") return _currentMouseState.RightButton == ButtonState.Pressed;
            if (mouseButton == "middle mouse") return _currentMouseState.MiddleButton == ButtonState.Pressed;
            else return false;
        }

        #endregion

        #region GamePad Events
        public bool WasGamePadPressed(Buttons button, PlayerIndex[] playernums = null)
        {
            if (playernums == null) playernums = new PlayerIndex[1] { 0 };
            if (button != 0)
            {
                foreach (PlayerIndex playernum in playernums)
                {
                    if (_previousGamePadStates[(int)playernum].IsButtonUp(button) && _currentGamePadStates[(int)playernum].IsButtonDown(button)) return true;
                }
            }
            return false;
        }
        public bool WasGamePadPressed(string button, int[] playernums = null)
        {
            try
            {
                if (playernums == null) playernums = new int[1] { 0 };
                return WasGamePadPressed((Buttons)Enum.Parse(typeof(Buttons), button), Array.ConvertAll(playernums, item => (PlayerIndex)item));
            }
            catch
            {
                Debug.WriteLine($"Game Pad Button '{button}' was not found");
                return false;
            }
        }
        public bool WasGamePadReleased(Buttons button, PlayerIndex[] playernums = null)
        {
            if (playernums == null) playernums = new PlayerIndex[1] { 0 };
            if (button != 0)
            {
                foreach (PlayerIndex playernum in playernums)
                {
                    if (_previousGamePadStates[(int)playernum].IsButtonDown(button) && _currentGamePadStates[(int)playernum].IsButtonUp(button)) return true;
                }
            }
            return false;
        }
        public bool WasGamePadReleased(string button, int[] playernums = null)
        {
            try
            {
                if (playernums == null) playernums = new int[1] { 0 };
                return WasGamePadReleased((Buttons)Enum.Parse(typeof(Buttons), button), Array.ConvertAll(playernums, item => (PlayerIndex)item));
            }
            catch
            {
                Debug.WriteLine($"Game Pad Button '{button}' was not found");
                return false;
            }
        }
        public bool IsGamePadButtonPressed(Buttons button, PlayerIndex[] playernums = null)
        {
            if (playernums == null) playernums = new PlayerIndex[1] { 0 };
            if (button != 0)
            {
                foreach (PlayerIndex playernum in playernums)
                {
                    if (_currentGamePadStates[(int)playernum].IsButtonDown(button)) return true;
                }
            }
            return false;
        }
        public bool IsGamePadButtonPressed(string button, int[] playernums = null)
        {
            try
            {
                if (playernums == null) playernums = new int[1] { 0 };
                return IsGamePadButtonPressed((Buttons)Enum.Parse(typeof(Buttons), button), Array.ConvertAll(playernums, item => (PlayerIndex)item));
            }
            catch
            {
                Debug.WriteLine($"Game Pad Button '{button}' was not found");
                return false;
            }
        }
        public float GetJoyStickAxis(string axis, int playernum = 0, bool raw = false)
        {
            float stickPos = 0;
            if (axis == "Left X") stickPos = _currentGamePadStates[playernum].ThumbSticks.Left.X;
            else if (axis == "Left Y") stickPos = _currentGamePadStates[playernum].ThumbSticks.Left.Y;
            else if (axis == "Right X") stickPos = _currentGamePadStates[playernum].ThumbSticks.Right.X;
            else if (axis == "Right Y") stickPos = _currentGamePadStates[playernum].ThumbSticks.Right.Y;
            else
            {
                Debug.WriteLine($"Game Pad Thumb Stick '{axis}' was not found");
                stickPos = 0;
                
            }
            if (raw)
            {
                if (stickPos > .25) stickPos = 1;
                else if (stickPos < -.25) stickPos = -1;
                else stickPos = 0;
            }
            return stickPos;
        }

        #endregion

        public float GetAxis(string axisName, bool raw = false)
        {
            if(_axes.TryGetValue(axisName, out Axis axis))
            {
                float speed = 1;
                if (raw) axis.CurrentValue = 0;
                else speed = axis.axisSpeed;

                // Positive buttons
                if (IsKeyDown(axis.PositiveKeyButton) || IsKeyDown(axis.AltPositiveKeyButton) ||
                    IsMousePressed(axis.PositiveMouseButton) || IsMousePressed(axis.AltPositiveMouseButton) ||
                    IsGamePadButtonPressed(axis.PositiveGamePadButton, axis.PlayerIndex) ||
                    IsGamePadButtonPressed(axis.AltPositiveGamePadButton, axis.PlayerIndex)) axis.CurrentValue += speed;
                else if (axis.CurrentValue > 0 && !raw)
                {
                    axis.CurrentValue -= speed * 5;
                    if (axis.CurrentValue < 0) axis.CurrentValue = 0;
                }

                // Negative buttons
                if (IsKeyDown(axis.NegativeKeyButton) || IsKeyDown(axis.AltNegativeKeyButton) ||
                    IsMousePressed(axis.NegativeMouseButton) || IsMousePressed(axis.AltNegativeMouseButton) ||
                    IsGamePadButtonPressed(axis.NegativeGamePadButton, axis.PlayerIndex) ||
                    IsGamePadButtonPressed(axis.AltNegativeGamePadButton, axis.PlayerIndex)) axis.CurrentValue -= speed;
                else if (axis.CurrentValue < 0 && !raw)
                {
                    axis.CurrentValue += speed * 5;
                    if (axis.CurrentValue > 0) axis.CurrentValue = 0;
                }

                // Joystick Axes
                float joyAxisVal = GetJoyStickAxis(axis.JoystickAxis);
                float altJoyAxisVal = GetJoyStickAxis(axis.AltJoystickAxis);
                if (joyAxisVal != 0) axis.CurrentValue = joyAxisVal;
                if (altJoyAxisVal != 0) axis.CurrentValue = altJoyAxisVal;

                return Math.Max(-1, Math.Min(axis.CurrentValue, 1));
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

