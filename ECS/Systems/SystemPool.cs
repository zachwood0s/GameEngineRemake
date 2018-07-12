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

        private string _poolName;

        public int ExecuteSystemCount => _executeSystems.Count;
        public int InitializeSystemCount => _initializeSystems.Count;
        public int OtherSystemCount => _otherSystems.Count;

        public string PoolName => _poolName;
        
        public SystemPool(string poolName)
        {
            _poolName = poolName;
            _executeSystems = new List<IExecuteSystem>();
            _initializeSystems = new List<IInitializeSystem>();
            _otherSystems = new List<ISystem>();
        }

        public virtual void Initialize()
        {
            foreach(IInitializeSystem iSystem in _initializeSystems)
            {
                iSystem.Initialize();
            }
        }
        public virtual void Execute()
        {
            foreach(IExecuteSystem eSystem in _executeSystems)
            {
                eSystem.Execute();
            }
        }
        public virtual void CleanUp()
        {

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
