using ECS.Systems.Interfaces;
using EngineCore.Systems.Global.InputManager;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.DebugSystems
{
    public class DebugModeToggleSystem : IExecuteSystem
    {
        private InputManager _inputManager;
        private Keys _debugKey;
        private Action _debugToggleAction;
        private bool _hasPressed;

        public DebugModeToggleSystem(InputManager inputManager, Keys debugKey, Action debugToggleAction)
        {
            _inputManager = inputManager;
            _debugKey = debugKey;
            _debugToggleAction = debugToggleAction;
        }
        public void Execute()
        {
            if (_inputManager.WasKeyPressed(_debugKey))
            {
                if (!_hasPressed)
                {
                    _debugToggleAction?.Invoke();
                }
                _hasPressed = true;
            }
            if (_inputManager.WasKeyReleased(_debugKey))
            {
                _hasPressed = false;
            }
        }
    }
}
