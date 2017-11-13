using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Components
{
    public static class ComponentPool
    {
        private static List<Type> _components = new List<Type>();

        public static int GetComponentIndex<T>() where T : IComponent
        {
            for(int i = 0; i<_components.Count; i++)
            {
                if (_components[i] == typeof(T)) return i;
            }
            return -1;
        }
        public static int GetComponentIndex(Type compType)
        {
            for(int i = 0; i<_components.Count; i++)
            {
                if (_components[i] == compType) return i;
            }
            return -1;
        }

        public static void RegisterComponent<T>() where T: IComponent
        {
            _components.Add(typeof(T));
        }

        public static void RegisterComponent(Type compType)
        {
            _components.Add(compType);
        }
    }
}
