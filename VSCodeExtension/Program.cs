using ECS.Attributes;
using EngineCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VSCodeExtension
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            var assemblies = LoadAssemblies();
            List<object> components = new List<object>();
            foreach (var assembly in assemblies)
            {
                foreach (Type t in GetTypesWithComponentAttribute(assembly))
                {
                    components.Add(Activator.CreateInstance(t));
                }
            }
            string json = JsonConvert.SerializeObject(components, Formatting.Indented, jsonSerializerSettings);
            File.WriteAllText("componentTypes.json", json);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        static List<Assembly> LoadAssemblies()
        {
            var fileNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("settings.json"));
            List<Assembly> allAssemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                if (fileNames.Exists(name => dll.Contains(name)))
                {
                    allAssemblies.Add(Assembly.LoadFile(dll));
                }
            }
            return allAssemblies;
        }
        static IEnumerable<Type> GetTypesWithComponentAttribute(Assembly assembly)
        {
            Console.WriteLine(assembly);
            List<Type> types = new List<Type>();
            var loadedTypes = GetLoadableTypes(assembly);
            foreach (Type type in loadedTypes)
            {
                if (type.GetCustomAttributes(typeof(ComponentAttribute), true).Length > 0)
                {
                    types.Add(type);
                }
            }
            return types;
        }
        static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
