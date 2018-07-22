using ECS.Components;
using ECS.Entities;
using ECS.Systems.Interfaces;
using EngineCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.EntityBuilderLoader
{
    public class EntityBuilderLoader: IInitializeSystem
    {

        private Dictionary<string, EntityBuilder> _entityBuilders;

        public string RootDirectory { get; set; }

        public EntityBuilderLoader(Dictionary<string, EntityBuilder> entityBuilders)
        {
            _entityBuilders = entityBuilders;
        }
        public void Initialize()
        {
            
            var list = JsonConvert.DeserializeObject<List<BuilderConstruct>>(File.ReadAllText("./"+RootDirectory+"/entity.json"));
            Console.WriteLine(list);
            foreach(var builderConstruct in list)
            {
                EntityBuilder newBuilder = _LoadBuilder(builderConstruct);
                _entityBuilders.Add(builderConstruct.Name, newBuilder);
            }
        }

        private EntityBuilder _LoadBuilder(BuilderConstruct builderConstruct)
        {
            EntityBuilder builder = _GetBuilder(builderConstruct);
            if (builderConstruct.Components != null)
            {
                foreach (var componentConstruct in builderConstruct.Components)
                {
                    ICopyableComponent component = _LoadComponent(componentConstruct);
                    if (component != null)
                    {
                        builder.With(component);
                    }
                }
            }
            return builder;
        }

        private EntityBuilder _GetBuilder(BuilderConstruct builderConstruct)
        {
            if(builderConstruct.Extends != null)
            {
                if (_entityBuilders.TryGetValue(builderConstruct.Extends, out EntityBuilder existing))
                {
                    return _RemoveComponentsFromExistingBuilder(builderConstruct, existing);
                }
                else{
                    Debug.WriteLine($"Attempted to extend entity builder {builderConstruct.Extends}" +
                        " but no such entity builder was found");
                }
            }
            return new EntityBuilder(); 
        }

        private EntityBuilder _RemoveComponentsFromExistingBuilder(BuilderConstruct builderConstruct, EntityBuilder existing)
        {
            EntityBuilder builder = existing.Copy();
            if(builderConstruct.Removes != null)
            {
                foreach(string removeTypeString in builderConstruct.Removes)
                {
                    Type removeType = Type.GetType(removeTypeString);
                    builder.Without(removeType);
                }
            }
            return builder;
        }

        private ICopyableComponent _LoadComponent(ComponentConstruct componentConstruct)
        {
            Type compType = Type.GetType(componentConstruct.Type);
            try
            {
                IComponent comp = (IComponent)componentConstruct.Data.ToObject(compType);
                if(comp is ICopyableComponent copyable)
                {
                    return copyable;
                }
                else
                {
                    Debug.WriteLine($"Component type {componentConstruct.Type} must implement" +
                        " the IComponentCopyable interface to be loaded from a file");
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Failed to create component from type {componentConstruct.Type}.");
                Debug.WriteLine($"Message given: {e.Message}.");
            }
            return null;
        }

    }

    class BuilderConstruct
    {
        public string Name { get; set; }
        public string Extends { get; set; }
        public List<ComponentConstruct> Components { get; set; }
        public List<string> Removes { get; set; }
    }
    class ComponentConstruct
    {
        public string Type { get; set; }
        public JObject Data { get; set; }
    }
}
