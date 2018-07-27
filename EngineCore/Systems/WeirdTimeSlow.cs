using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using EngineCore.Components;
using ECS.Matching;
using ECS.Entities;
using EngineCore.Systems.Global.InputManager;
using ECS.Systems.Interfaces;

namespace EngineCore.Systems
{
    public class TimeSlow : IExecuteSystem
    {
        private InputManager _inputManager;
        private ThreadedSystemPool _updatePool;

        public TimeSlow(InputManager inputManager, ThreadedSystemPool updatePool)
        {
            _inputManager = inputManager;
            _updatePool = updatePool;
        }
        public void Execute()
        {
            if (_updatePool.TargetFPS < 500)
                _updatePool.TargetFPS += 5 * (int)_inputManager.GetAxis("time");
            else
                _updatePool.TargetFPS = 499;
        }

    }
} 