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
    class EntityBuilderLoader: IInitializeSystem
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
            foreach(var item in list)
            {
                EntityBuilder newBuilder = new EntityBuilder();
                foreach(var component in item.Components)
                {
                    Type compType = Type.GetType(component.Type);
                    try
                    {
                        IComponent comp = (IComponent)component.Data.ToObject(compType);
                        if(comp is ICopyableComponent copyable)
                        {
                            newBuilder.With(copyable); 
                        }
                        else
                        {
                            Debug.WriteLine($"Component type {component.Type} must implement" +
                                " the IComponentCopyable interface to be loaded from a file");
                        }
                    }
                    catch
                    {
                        Debug.WriteLine($"Failed to create component from type {component.Type}");
                    }
                }
            }
        }

        private static Func<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(Func<T>), newExp, param);

            //compile it
            Func<T> compiled = (Func<T>)lambda.Compile();
            return compiled;
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
