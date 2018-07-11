using ECS.Components;
using ECS.Components.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Entities
{
    public class EntityBuilder : IEntityBuilder
    {
        //private List<IComponent> _components; //I think the solution might be to only keep the types
        // or the type indicies. This would be more where my creation function would come into play i guess
        //private List<int> _componentTypeIndicies;
        private List<Type> _componentTypes;
        private List<int> _componentTypeIndicies;
        private List<Func<IComponent>> _creationFunctions;
        private Entity _entity;

        public EntityBuilder()
        {
            _creationFunctions = new List<Func<IComponent>>();
            _componentTypeIndicies = new List<int>();
            _componentTypes = new List<Type>();
        }

        public EntityBuilder With<T>() where T : IComponentHasDefault
        {
            if(!_componentTypes.Contains(typeof(T)))
            {
                bool added = _AddTypeIndice(typeof(T));
                if(added) _componentTypes.Add(typeof(T));
            }
            return this;
        }

        public EntityBuilder With<T>(Func<T> creationFunc) where T: IComponent
        {
            bool added = _AddTypeIndice(creationFunc.Method.ReturnParameter.ParameterType);
            //Okay so why does this line work, and not
            // _creationFunction.Add(creationFunc); ??
            // why would the extra function call be necessary?
            if(added) _creationFunctions.Add(() => creationFunc());
            return this;
        }

        private bool _AddTypeIndice(Type t)
        {

            int newCompIndex = ComponentPool.GetComponentIndex(t);

            if (newCompIndex < 0)
                throw new UnregisteredComponentException($"Component of type {t} has not been registered with the ComponentPool.");

            if(!_componentTypeIndicies.Contains(newCompIndex))
            {
                _componentTypeIndicies.Add(newCompIndex);
                return true;
            }
            return false;
        }

        public Entity Build(Scene scene)
        {
            List<IComponent> components = new List<IComponent>();

            foreach(Type t in _componentTypes)
            {
                var newComp = Activator.CreateInstance(t);

                if(newComp is IComponentHasDefault comp)
                {
                    comp.SetDefaults();
                    components.Add(comp);
                }
            }

            foreach(Func<IComponent> function in _creationFunctions)
            {
                components.Add(function());
            }
            //It would be really nice if the Entity no longer had to deal with the subscriptions
            //and only the entity group manager dealt with it. 


            _entity = new Entity(components, _componentTypeIndicies);
            scene.AddEntity(_entity);
            return _entity;
        }

    }
}
