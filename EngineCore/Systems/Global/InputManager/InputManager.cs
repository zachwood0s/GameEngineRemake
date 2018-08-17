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
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }
        public bool IsKeyPressed(string keyName)
        {
            try
            {
                return IsKeyPressed((Keys)Enum.Parse(typeof(Keys), keyName));
            }
            catch
            {
                Debug.WriteLine($"Key '{keyName}' was not found");
                return false;
            }
        }

        #endregion

        # region Mouse Events

        public bool WasMousePressed(MouseButtons mouseButton)
        {
            if (mouseButton == MouseButtons.Left) return _currentMouseState.LeftButton == ButtonState.Pressed &&
                                                    _previousMouseState.LeftButton == ButtonState.Released;
            if (mouseButton == MouseButtons.Right) return _currentMouseState.RightButton == ButtonState.Pressed &&
                                                    _previousMouseState.RightButton == ButtonState.Released;
            if (mouseButton == MouseButtons.Middle) return _currentMouseState.MiddleButton == ButtonState.Pressed &&
                                                    _previousMouseState.MiddleButton == ButtonState.Released;
            else
            {
                Debug.WriteLine($"Mouse '{mouseButton}' does not exist");
                return false;
            }
        }
        public bool WasMouseReleased(MouseButtons mouseButton)
        {
            if (mouseButton == MouseButtons.Left) return _currentMouseState.LeftButton == ButtonState.Released && 
                                                    _previousMouseState.LeftButton == ButtonState.Pressed;
            if (mouseButton == MouseButtons.Right) return _currentMouseState.RightButton == ButtonState.Released &&
                                                    _previousMouseState.RightButton == ButtonState.Pressed;
            if (mouseButton == MouseButtons.Middle) return _currentMouseState.MiddleButton == ButtonState.Released &&
                                                    _previousMouseState.MiddleButton == ButtonState.Pressed;
            else
            {
                Debug.WriteLine($"Mouse '{mouseButton}' does not exist");
                return false;
            }
        }
        public bool IsMousePressed(MouseButtons mouseButton)
        {
            if (mouseButton == MouseButtons.Left) return _currentMouseState.LeftButton == ButtonState.Pressed;
            if (mouseButton == MouseButtons.Right) return _currentMouseState.RightButton == ButtonState.Pressed;
            if (mouseButton == MouseButtons.Middle) return _currentMouseState.MiddleButton == ButtonState.Pressed;
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
        public float GetJoyStickAxis(Joystick axis, PlayerIndex playernum = PlayerIndex.One, bool raw = false)
        {
            float stickPos = 0;
            if (axis == Joystick.LeftX) stickPos = _currentGamePadStates[(int)playernum].ThumbSticks.Left.X;
            else if (axis == Joystick.LeftY) stickPos = _currentGamePadStates[(int)playernum].ThumbSticks.Left.Y;
            else if (axis == Joystick.RightX) stickPos = _currentGamePadStates[(int)playernum].ThumbSticks.Right.X;
            else if (axis == Joystick.RightY) stickPos = _currentGamePadStates[(int)playernum].ThumbSticks.Right.Y;
            else if (axis != Joystick.None)
            {
                Debug.WriteLine($"Game Pad Thumb Stick '{axis}' was not found");
                return 0;
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

        #region Axis Events

        public float GetAxis(string axisName)
        {
            if(_axes.TryGetValue(axisName, out Axis axis))
            {
                float speed = 0;
                float invertVal = 1;
                if (axis.Invert) invertVal = -1;

                // Positive buttons
                if (IsKeyPressed(axis.PositiveKeyButton) || IsKeyPressed(axis.AltPositiveKeyButton) ||
                    IsMousePressed(axis.PositiveMouseButton) || IsMousePressed(axis.AltPositiveMouseButton) ||
                    IsGamePadButtonPressed(axis.PositiveGamePadButton, axis.PlayerIndex) ||
                    IsGamePadButtonPressed(axis.AltPositiveGamePadButton, axis.PlayerIndex)) speed += 1;

                // Negative buttons
                if (IsKeyPressed(axis.NegativeKeyButton) || IsKeyPressed(axis.AltNegativeKeyButton) ||
                    IsMousePressed(axis.NegativeMouseButton) || IsMousePressed(axis.AltNegativeMouseButton) ||
                    IsGamePadButtonPressed(axis.NegativeGamePadButton, axis.PlayerIndex) ||
                    IsGamePadButtonPressed(axis.AltNegativeGamePadButton, axis.PlayerIndex)) speed -= 1;

                // Joystick Axes
                float joyMax = 0;
                foreach(PlayerIndex playerIndex in axis.PlayerIndex)
                {
                    float joyAxisVal = GetJoyStickAxis(axis.JoystickAxis, playerIndex);
                    float altJoyAxisVal = GetJoyStickAxis(axis.AltJoystickAxis, playerIndex);
                    if (Math.Abs(joyMax) < Math.Abs(joyAxisVal)) joyMax = joyAxisVal;
                    if (Math.Abs(joyMax) < Math.Abs(altJoyAxisVal)) joyMax = joyAxisVal;
                }
                if (joyMax != 0) speed = joyMax;             
       
                return Math.Max(-1, Math.Min(speed*invertVal, 1));

            }
            else
            {
                Debug.WriteLine($"Axis '{axisName}' does not exist");
                return 0;
            }
        }
        public Vector2 GetAxesAsVector(string axis1, string axis2)
        {
            return new Vector2(GetAxis(axis1), GetAxis(axis2));
        }
        public bool GetAxisDown(string axisName)
        {
            if (_axes.TryGetValue(axisName, out Axis axis))
            {
                // Positive buttons
                if (WasKeyPressed(axis.PositiveKeyButton) || WasKeyPressed(axis.AltPositiveKeyButton) ||
                    WasMousePressed(axis.PositiveMouseButton) || WasMousePressed(axis.AltPositiveMouseButton) ||
                    WasGamePadPressed(axis.PositiveGamePadButton, axis.PlayerIndex) ||
                    WasGamePadPressed(axis.AltPositiveGamePadButton, axis.PlayerIndex)) return true;

                // Negative buttons
                if (WasKeyPressed(axis.NegativeKeyButton) || WasKeyPressed(axis.AltNegativeKeyButton) ||
                    WasMousePressed(axis.NegativeMouseButton) || WasMousePressed(axis.AltNegativeMouseButton) ||
                    WasGamePadPressed(axis.NegativeGamePadButton, axis.PlayerIndex) ||
                    WasGamePadPressed(axis.AltNegativeGamePadButton, axis.PlayerIndex)) return true;               
            }
            else
            {
                Debug.WriteLine($"Axis '{axisName}' does not exist");
            }
            return false;
        }
        public bool GetAxisReleased(string axisName)
        {
            if (_axes.TryGetValue(axisName, out Axis axis))
            {
                // Positive buttons
                if (WasKeyReleased(axis.PositiveKeyButton) || WasKeyReleased(axis.AltPositiveKeyButton) ||
                    WasMouseReleased(axis.PositiveMouseButton) || WasMouseReleased(axis.AltPositiveMouseButton) ||
                    WasGamePadReleased(axis.PositiveGamePadButton, axis.PlayerIndex) ||
                    WasGamePadReleased(axis.AltPositiveGamePadButton, axis.PlayerIndex)) return true;

                // Negative buttons
                if (WasKeyReleased(axis.NegativeKeyButton) || WasKeyReleased(axis.AltNegativeKeyButton) ||
                    WasMouseReleased(axis.NegativeMouseButton) || WasMouseReleased(axis.AltNegativeMouseButton) ||
                    WasGamePadReleased(axis.NegativeGamePadButton, axis.PlayerIndex) ||
                    WasGamePadReleased(axis.AltNegativeGamePadButton, axis.PlayerIndex)) return true;
            }
            else
            {
                Debug.WriteLine($"Axis '{axisName}' does not exist");
            }
            return false;
        }
        public bool GetAxisPressed(string axisName)
        {
            return Convert.ToBoolean(GetAxis(axisName));
        }
        public bool GetPositiveAxisPressed(string axisName)
        {
            if (GetAxis(axisName) > 0) return true;
            return false;
        }
        public bool GetNegativeAxisPressed(string axisName)
        {
            if (GetAxis(axisName) < 0) return true;
            return false;
        }
        public void AddAxis(string axisName, Axis axis)
        {
            _axes.Add(axisName, axis);
        }

        #endregion

    }

}

