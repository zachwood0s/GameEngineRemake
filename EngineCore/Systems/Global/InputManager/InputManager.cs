using ECS.Systems.Interfaces;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.InputManager
{
    public class InputManager : IExecuteSystem
    {
        private Dictionary<string, Axis> _axes;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        public InputManager()
        {
            _axes = new Dictionary<string, Axis>();
        }

        public void Execute()
        {
            _previousKeyboardState = _currentKeyboardState;

            _currentKeyboardState = Keyboard.GetState();
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
                return axis.GetValue(_currentKeyboardState);
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

    public class Axis
    {
        private Keys _positiveButton,
                     _negativeButton,
                     _altPositiveButton,
                     _altNegativeButton;

        public Axis(Keys positiveButton, Keys altPositiveButton, Keys negativeButton, Keys altNegativeButton)
        {
            _positiveButton = positiveButton;
            _altPositiveButton = altPositiveButton;
            _negativeButton = negativeButton;
            _altNegativeButton = altNegativeButton;
        }

        public float GetValue(KeyboardState keyboardState)
        {
            float value = 0;
            if (keyboardState.IsKeyDown(_positiveButton) || keyboardState.IsKeyDown(_altPositiveButton)) value += 1;
            if (keyboardState.IsKeyDown(_negativeButton) || keyboardState.IsKeyDown(_altNegativeButton)) value -= 1;

            value = Math.Max(-1, Math.Min(value, 1));
            return value;
        }
    }
}

