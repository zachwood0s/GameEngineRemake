using ECS.Components;
using ECS.Components.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Entities
{
    /// <summary>
    /// TODO: 
    /// I think I'd like to change without to returning a copy and removing
    /// and then add a separate remove function to handle just removing.
    /// </summary>
    public class EntityBuilder : IEntityBuilder
    {
        //private List<IComponent> _components; //I think the solution might be to only keep the types
        // or the type indicies. This would be more where my creation function would come into play i guess
        //private List<int> _componentTypeIndicies;
        private List<Type> _componentTypes;
        private List<int> _componentTypeIndicies;
        private List<ICopyableComponent> _componentTemplates;
        private List<Func<IComponent>> _creationFunctions;

        /// <summary>
        /// This is necessary because I found it quite hard to figure out
        /// exaclty what the return types were of the creation functions.
        /// Because they had been cast into an IComponent it was hard to tell 
        /// exactly what type of component they were
        /// </summary>
        private List<int> _creationFunctionReturnTypes;
        private Entity _entity;

        public EntityBuilder()
        {
            _creationFunctions = new List<Func<IComponent>>();
            _componentTypeIndicies = new List<int>();
            _componentTypes = new List<Type>();
            _componentTemplates = new List<ICopyableComponent>();
            _creationFunctionReturnTypes = new List<int>();
        }

        private EntityBuilder(List<Func<IComponent>> funcs,
                              List<int> indicies,
                              List<Type> types,
                              List<ICopyableComponent> templates,
                              List<int> returnTypes)
        {
            _creationFunctions = new List<Func<IComponent>>(funcs);
            _componentTypeIndicies = new List<int>(indicies);
            _componentTypes = new List<Type>(types);
            _componentTemplates = new List<ICopyableComponent>(templates);
            _creationFunctionReturnTypes = new List<int>(returnTypes);
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
            bool added = _AddTypeIndice<T>();
            //Okay so why does this line work, and not
            // _creationFunction.Add(creationFunc); ??
            // why would the extra function call be necessary?
            if (added)
            {
                _creationFunctions.Add(() => creationFunc());
                _creationFunctionReturnTypes.Add(ComponentPool.GetComponentIndex<T>());
            }
            return this;
        }

        public EntityBuilder With(ICopyableComponent comp)
        {
            bool added = _AddTypeIndice(comp.GetType());

            if (added)
            {
                _componentTemplates.Add(comp);
            }
            return this;
        }

        public EntityBuilder Without<T>() where T : IComponent
        {
            Type t = typeof(T);
            int compIndex = ComponentPool.GetComponentIndex(t);

            if (compIndex < 0)
                throw new UnregisteredComponentException($"Component of type {t} has not been registered with the ComponentPool.");

            if (_componentTypeIndicies.Contains(compIndex))
            {
                _componentTypeIndicies.Remove(compIndex);

                if (_componentTypes.Contains(t)) _componentTypes.Remove(t);

                _RemoveFromFunctions(compIndex);
                _RemoveFromTemplates(t);

            }
            return this;
        }

        private void _RemoveFromFunctions(int compIndex)
        {
            int functionIndex = _creationFunctionReturnTypes.IndexOf(compIndex);
            if (functionIndex >= 0)
            {
                _creationFunctionReturnTypes.RemoveAt(functionIndex);
                _creationFunctions.RemoveAt(functionIndex);
            }
        }

        private void _RemoveFromTemplates(Type compType)
        {
            _componentTemplates.RemoveAll(c => c.GetType() == compType);
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

        private bool _AddTypeIndice<T>() where T: IComponent
        {
            return _AddTypeIndice(typeof(T));
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

            foreach(ICopyableComponent comp in _componentTemplates)
            {
                components.Add(comp.Copy());
            }

            _entity = new Entity(components, _componentTypeIndicies);
            scene.AddEntity(_entity);
            return _entity;
        }

        public EntityBuilder Copy()
        {
            return new EntityBuilder(
                _creationFunctions, 
                _componentTypeIndicies, 
                _componentTypes, 
                _componentTemplates,
                _creationFunctionReturnTypes
                );
        }
    }
}
