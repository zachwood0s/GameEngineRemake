using ECS.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global
{
    public static class LoaderHelper
    {
        public static IComponent LoadComponent(ComponentConstruct componentConstruct)
        {
            Type compType = Type.GetType(componentConstruct.Type);
            try
            {
                IComponent comp = (IComponent)componentConstruct.Data.ToObject(compType);
                return comp;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to create component from type {componentConstruct.Type}.");
                Debug.WriteLine($"Message given: {e.Message}.");
            }
            return null;
        }

        public static ICopyableComponent LoadCopyableComponent(ComponentConstruct componentConstruct)
        {
            IComponent comp = LoadComponent(componentConstruct);
            if(comp is ICopyableComponent copyable)
            {
                return copyable;
            }
            else
            {
                Debug.WriteLine($"Component type {componentConstruct.Type} must implement" +
                    " the IComponentCopyable interface to be loaded from a file");
            }
            return null;
        }

        //public static ICopyableComponent 
    }


    public class SceneConstruct
    {
        public string Name { get; set; }
        public List<string> SystemPools { get; set; }
        public List<EntityConstruct> Entities { get; set; }
    }

    public class EntityConstruct
    {
        public string Extends { get; set; }
        public List<string> Removes { get; set; }
        public List<ComponentConstruct> Components { get; set; }
    }

    public class BuilderConstruct
    {
        public string Name { get; set; }
        public string Extends { get; set; }
        public List<ComponentConstruct> Components { get; set; }
        public List<string> Removes { get; set; }
    }

    public class ComponentConstruct
    {
        public string Type { get; set; }
        public JObject Data { get; set; }
    }
}
