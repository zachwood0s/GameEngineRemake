using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    public class SystemPool
    {
        private List<IExecuteSystem> _executeSystems;
        private List<IInitializeSystem> _initializeSystems;
        private List<ISystem> _otherSystems;

        public List<IExecuteSystem> ExecuteSystems => _executeSystems;
        public List<IInitializeSystem> InitializeSystems => _initializeSystems;
        public List<ISystem> OtherSystems => _otherSystems;
        
        public SystemPool()
        {
            _executeSystems = new List<IExecuteSystem>();
            _initializeSystems = new List<IInitializeSystem>();
            _otherSystems = new List<ISystem>();
        }

        public void Initialize()
        {
            foreach(IInitializeSystem iSystem in _initializeSystems)
            {
                iSystem.Initialize();
            }
        }
        public void Execute()
        {
            foreach(IExecuteSystem eSystem in _executeSystems)
            {
                eSystem.Execute();
            }
        }
        public SystemPool Register(ISystem system)
        {
            if(system is IExecuteSystem eSystem)
            {
                _executeSystems.Add(eSystem);
            }
            if(system is IInitializeSystem iSystem)
            {
                _initializeSystems.Add(iSystem);
            }
            return this;
        }

        public T GetSystem<T>() where T: class, ISystem
        {

            foreach (IExecuteSystem e in _executeSystems)
            {
                if(e is T tSys)
                {
                    return tSys;
                }
            }
            foreach (IInitializeSystem e in _initializeSystems)
            {
                if(e is T tSys)
                {
                    return tSys;
                }
            }
            foreach (ISystem e in _otherSystems)
            {
                if(e is T tSys)
                {
                    return tSys;
                }
            }

            return null;
        }
    }
}
