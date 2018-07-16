using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems.SystemPools
{
    public class SystemPoolBuilder
    {
        private List<Func<Scene, ISystem>> _creationFuncs;
        private List<Type> _creationFuncsTypes;
        private int _targetFps;
        private bool _isThreaded;
        private string _name;

        public SystemPoolBuilder(string name)
        {
            _creationFuncs = new List<Func<Scene, ISystem>>();
            _creationFuncsTypes = new List<Type>();
            _name = name;
        }

        private SystemPoolBuilder(List<Func<Scene, ISystem>> funcs, int targetFps, bool isThreaded, List<Type> types, string name)
        {
            _creationFuncs = new List<Func<Scene, ISystem>>(funcs);
            _targetFps = targetFps;
            _isThreaded = isThreaded;
            _creationFuncsTypes = new List<Type>(types);
            _name = name;
        }

        public SystemPoolBuilder With<T>(Func<Scene, T> func) where T: ISystem
        {
            _creationFuncs.Add((Scene s) => func(s));
            _creationFuncsTypes.Add(typeof(T));
            return this;
        }

        public SystemPoolBuilder Remove<T>() where T : ISystem
        {
            return Remove<T>(0);
        }

        public SystemPoolBuilder Remove<T>(int startIndex) where T : ISystem
        {
            int index = _creationFuncsTypes.IndexOf(typeof(T), startIndex);
            if(index < 0) throw new ArgumentOutOfRangeException("T", "The system type provided has not been added to the build list");

            _creationFuncs.RemoveAt(index);
            _creationFuncsTypes.RemoveAt(index);

            return this;
        }

        public SystemPoolBuilder RemoveAll<T>() where T : ISystem
        {
            Type t = typeof(T);
            int index = _creationFuncsTypes.IndexOf(t);
            while(index >= 0)
            {
                _creationFuncs.RemoveAt(index);
                _creationFuncsTypes.RemoveAt(index);
                index = _creationFuncsTypes.IndexOf(t, index);
            }

            return this;
        }

        public SystemPoolBuilder Without<T>() where T : ISystem
        {
            return Copy().Remove<T>();
        }

        public SystemPoolBuilder Without<T>(int startIndex) where T : ISystem
        {
            return Copy().Remove<T>(startIndex);
        }

        public SystemPoolBuilder WithoutAll<T>() where T : ISystem
        {
            return Copy().RemoveAll<T>();
        }

        public SystemPoolBuilder WithFPS(int fps)
        {
            if (fps <= 0) throw new ArgumentOutOfRangeException("fps", "The target fps must be greater than 0");
            _targetFps = fps;
            _isThreaded = true;
            return this;
        }

        /// <summary>
        /// Removes the FPS requirement on the current builder
        /// </summary>
        /// <example>
        /// builder.RemoveFPS();
        /// </example>
        /// <returns>The same builder without FPS</returns>
        public SystemPoolBuilder RemoveFPS()
        {
            _isThreaded = false;
            return this;
        }

        /// <summary>
        /// Creates a copy of the current builder and removes the
        /// FPS from the new builder.
        /// </summary>
        /// <example>
        /// newBuilder = oldBuilder.WithoutFPS();
        /// </example>
        /// <returns>A copy of the builder without FPS</returns>
        public SystemPoolBuilder WithoutFPS()
        {
            return Copy().RemoveFPS();
        }

        public SystemPoolBuilder Copy()
        {
            return new SystemPoolBuilder(_creationFuncs, _targetFps, _isThreaded, _creationFuncsTypes, _name);
        }

        public SystemPool Build(Scene scene, string suffix = "")
        {

            SystemPool newPool;
            if (_isThreaded) newPool = new ThreadedSystemPool(_name + suffix, _targetFps);
            else newPool = new SystemPool(_name + suffix);

            foreach(var creationFunc in _creationFuncs)
            {
                newPool.Register(creationFunc(scene));
            }
            return newPool;
        }
    }
}
