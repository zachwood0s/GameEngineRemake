using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    /// <summary>
    /// A system pool holds a group of systems, whether they be
    /// execute/initialize systems. When execute is called, it will
    /// call execute on all of it's execute systems and the same for
    /// initialize systems.
    /// </summary>
    public class SystemPool
    {
        private List<IExecuteSystem> _executeSystems;
        private List<IInitializeSystem> _initializeSystems;
        private List<ISystem> _otherSystems;
        protected long _currentFps;
        protected double _avgFps = 0;

        private string _poolName;

        private int _frameCount;
        private DateTime _lastFrameReadTime;

        public long CurrentFps => _currentFps;
        public double AvgFps => _avgFps;

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

        /// <summary>
        /// Run the Initialize method for all stored IInitializeSystems
        /// </summary>
        public virtual void Initialize()
        {
            foreach(IInitializeSystem iSystem in _initializeSystems)
            {
                iSystem.Initialize();
            }
        }

        /// <summary>
        /// Run the Execute method for all stored IExecuteSystems
        /// </summary>
        public virtual void Execute()
        {
            _frameCount++;
            if((DateTime.Now - _lastFrameReadTime).TotalSeconds >= 1)
            {
                _currentFps = _frameCount;
                _avgFps += _currentFps;
                _avgFps /= 2;

                _frameCount = 0;
                _lastFrameReadTime = DateTime.Now;
            }
            foreach(IExecuteSystem eSystem in _executeSystems)
            {
                eSystem.Execute();
            }
        }
        public virtual void CleanUp()
        {

        }

        /// <summary>
        /// Adds the provided system to the SystemPool
        /// </summary>
        /// <param name="system">The system to add</param>
        /// <returns>Returns itself so that method chaining is possibe</returns>
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

        /// <summary>
        /// Get the system of the specified type that's stored within
        /// this SystemPool
        /// </summary>
        /// <typeparam name="T">The type of system to retrieve</typeparam>
        /// <returns>The system that was found</returns>
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
