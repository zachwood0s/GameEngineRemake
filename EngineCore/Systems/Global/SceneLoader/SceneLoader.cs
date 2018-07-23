using ECS;
using ECS.Components;
using ECS.Entities;
using ECS.Systems.Interfaces;
using ECS.Systems.SystemPools;
using EngineCore.Systems.Global.EntityBuilderLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.SceneLoader
{
    class SceneLoader: IInitializeSystem
    {
        private Dictionary<string, Scene> _scenes;
        private Dictionary<string, SystemPoolBuilder> _systemPoolBuilders;
        private Dictionary<string, EntityBuilder> _entityBuilders;

        public string RootDirectory { get; set; }

        public SceneLoader(Dictionary<string, Scene> scenes,
                           Dictionary<string, SystemPoolBuilder> systemPoolBuilders,
                           Dictionary<string, EntityBuilder> entityBuilders)
        {
            _scenes = scenes;
            _systemPoolBuilders = systemPoolBuilders;
            _entityBuilders = entityBuilders;
        }
        public void Initialize()
        {
            foreach (string file in Directory.EnumerateFiles("./" + RootDirectory, "*.json", SearchOption.AllDirectories))
            {
                _LoadFile(file);
            }
        }

        private void _LoadFile(string file)
        {
            SceneConstruct sceneConstruct = JsonConvert.DeserializeObject<SceneConstruct>(File.ReadAllText(file));
            Scene newScene = new Scene();
            _LoadSystemPools(sceneConstruct, newScene);
            _LoadEntities(sceneConstruct, newScene);
            _scenes.Add(sceneConstruct.Name, newScene);
        }

        private void _LoadSystemPools(SceneConstruct sceneConstruct, Scene newScene)
        {
            if(sceneConstruct.SystemPools != null)
            {
                foreach(string systemPoolName in sceneConstruct.SystemPools)
                {
                    if(_systemPoolBuilders.TryGetValue(systemPoolName, out SystemPoolBuilder builder))
                    {
                        newScene.AddSystemPoolFromBuilder(builder);
                    }
                    else
                    {
                        Debug.WriteLine($"System pool {systemPoolName} does not exist");
                    }
                }
            }
        }

        private void _LoadEntities(SceneConstruct sceneConstruct, Scene newScene)
        {
            if(sceneConstruct.Entities == null || sceneConstruct.Entities.Count == 0)
            {
                Debug.WriteLine($"WARNING: Scene {sceneConstruct.Name} has no entities in it!");
                return;
            }

            foreach(EntityConstruct entityConstruct in sceneConstruct.Entities)
            {
                Entity newEntity = _GetEntity(entityConstruct, newScene);

                _AddComponentsToEntity(entityConstruct, newEntity);
            }
        }

        private Entity _GetEntity(EntityConstruct entityConstruct, Scene newScene)
        {
            if(entityConstruct.Extends != null)
            {
                if(_entityBuilders.TryGetValue(entityConstruct.Extends, out EntityBuilder builder))
                {
                    Entity newEntity = newScene.CreateEntityFromBuilder(builder);
                    _RemoveComponentsFromEntity(entityConstruct, newEntity);
                    return newEntity;
                }
                else
                {
                    Debug.WriteLine($"Entity builder with name {entityConstruct.Extends} does not exist");
                }
            }
            return newScene.CreateEntity();
        }

        private void _RemoveComponentsFromEntity(EntityConstruct entityConstruct, Entity newEntity)
        {
            if(entityConstruct.Removes != null)
            {
                foreach(string removeTypeName in entityConstruct.Removes)
                {
                    Type removeType = Type.GetType(removeTypeName);
                    newEntity.Remove(removeType);
                }
            }
        }

        private void _AddComponentsToEntity(EntityConstruct entityConstruct, Entity newEntity)
        {
            if(entityConstruct.Components != null)
            {
                foreach(ComponentConstruct componentConstruct in entityConstruct.Components)
                {
                    IComponent component = LoaderHelper.LoadComponent(componentConstruct);
                    if (component != null)
                    {
                        newEntity.With(component);
                    }
                }
            }
        }
    }

}
