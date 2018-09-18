using ECS.Attributes;
using EngineCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
        private const string COMP_PREFIX = "Component";
        private const string ENTITY_PREFIX = "Entities";
        class Snippet
        {
            [JsonProperty]
            public static string scope = "json";
            [JsonProperty]
            public List<string> body { get; set; } = new List<string>();
            [JsonProperty]
            public string prefix = "";
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: VSCodeExtension {settingsFilePath} {outLocation}");
                return;
            }

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var assemblies = LoadAssemblies(args[0]);

            var components = CreateComponents(assemblies);
            var snippets = CreateComponentSnippets(components, jsonSerializerSettings);
            snippets.Add("Entity", CreateEntitySnippet(assemblies));

            string json = JsonConvert.SerializeObject(snippets, Formatting.Indented, jsonSerializerSettings);
            File.WriteAllText(args[1], json);
        }

        #region Components
        static Dictionary<string, Snippet> CreateComponentSnippets(Dictionary<Type, JObject> components, JsonSerializerSettings settings)
        {
            var dict = new Dictionary<string, Snippet>();
            foreach (var component in components)
            {
                string ser = JsonConvert.SerializeObject(component.Value, Formatting.Indented, settings);
                var snip = new Snippet()
                {
                    prefix = COMP_PREFIX,
                    body = new List<string>(ser.Replace("\r", "").Split('\n'))
                };
                snip.body.Insert(1, $"  \"$$type\": \"{component.Key.FullName}\",");
                dict.Add(component.Key.Name, snip);
            }
            return dict;
        }

        static Dictionary<Type, JObject> CreateComponents(IEnumerable<Assembly> assemblies)
        {
            var components = new Dictionary<Type, JObject>();
            foreach (var assembly in assemblies)
            {
                foreach (Type t in GetTypesWithComponentAttribute(assembly))
                {
                    var comp = Activator.CreateInstance(t);
                    var jObject = CreateSerializable(comp);
                    components.Add(comp.GetType(), jObject);
                }
            }
            return components;
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

        #endregion

        #region Entities

        static Snippet CreateEntitySnippet(IEnumerable<Assembly> assemblies)
        {
            Type type = assemblies
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.Name == "EntityConstruct")
                        .FirstOrDefault();
            var construct = Activator.CreateInstance(type);
            var jObject = CreateSerializable(construct);
            string json = jObject.ToString(Formatting.Indented);

            var snip = new Snippet()
            {
                prefix = ENTITY_PREFIX,
                body = new List<string>(json.Replace("\r", "").Split('\n'))
            };
            return snip;
        }


        #endregion

        static JObject CreateSerializable(object o)
        {
            var jObject = JObject.FromObject(o);

            int count = 1;
            foreach(var prop in jObject.Children().OfType<JProperty>())
            {
                Type propType = o.GetType().GetProperty(prop.Name).PropertyType;
                if (!prop.First.IsNullOrEmpty())
                {
                    if (prop.First.Type != JTokenType.Object)
                    {
                        prop.Value = new JRaw("${" + count + ":"+ prop.Value.ToString(Formatting.None) +"}");
                        count++;
                    }
                }
                else 
                {
                    if(propType == typeof(string))
                    {
                        prop.Value = new JRaw("${" + count + ":\"\"}");
                        count++;
                    }
                    else
                    {
                        try
                        {
                            object obj = Activator.CreateInstance(propType);
                            if(obj is IList list)
                            {
                                Type genericType = list.GetType().GetGenericArguments().Single();
                                var sampleObj = Activator.CreateInstance(genericType);
                                list.Add(sampleObj);
                            }
                            prop.Value = JToken.FromObject(obj);
                        }
                        catch (MissingMethodException e)
                        {

                        }
                    }
                   // prop.Value = new JRaw("${" + count + ":"+ propType.Name +"}");
                    //count++;
                }
            }
            return jObject;
        }

        static List<Assembly> LoadAssemblies(string settingsFileName)
        {
            var fileNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(settingsFileName));
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

    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
        public static bool IsNullOrEmpty(this JProperty token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
