using ECS.Attributes;
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
        public static IReadOnlyList<Type> Components => _components;

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
            RegisterComponent(typeof(T));
        }

        public static void RegisterComponent(Type compType)
        {
            if (!_components.Contains(compType))
            {
                _components.Add(compType);
            }
        }

        public static void RegisterAllComponents()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assemblies)
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if(type.GetCustomAttributes(typeof(ComponentAttribute), true).Length > 0)
                    {
                        RegisterComponent(type);
                    }
                }
            }
        }
    }
}
