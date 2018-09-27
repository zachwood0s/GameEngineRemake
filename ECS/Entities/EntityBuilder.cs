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
    /// The <see cref="EntityBuilder"/> class allows you to create a prototype 
    /// for an entity that can be created at a later time. This is very useful
    /// for creating a lot of entities of the same type with the same components
    /// and component starting values. 
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

        /// <summary>
        /// Adds a component of type T to the EntityBuilder if it hasn't
        /// already been added
        /// </summary>
        /// <typeparam name="T">Component type that must implement IComponentHasDefault</typeparam>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public EntityBuilder With<T>() where T : IComponentHasDefault
        {
            if(!_componentTypes.Contains(typeof(T)))
            {
                bool added = _AddTypeIndice(typeof(T));
                if(added) _componentTypes.Add(typeof(T));
            }
            return this;
        }

        /// <summary>
        /// Adds a creation function to the EntityBuilder. This function
        /// will be called when the builder is built and needs to return 
        /// a component that implements IComponent
        /// </summary>
        /// <typeparam name="T">Component type that is being returned. Must implement IComponent</typeparam>
        /// <param name="creationFunc">The funcion that is called at build time</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public EntityBuilder With<T>(Func<T> creationFunc) where T: IComponent
        {
            bool added = _AddTypeIndice<T>();
            if (added)
            {
                //Okay so why does this line work, and not
                // _creationFunction.Add(creationFunc); ??
                // why would the extra function call be necessary?
                _creationFunctions.Add(() => creationFunc());
                _creationFunctionReturnTypes.Add(ComponentPool.GetComponentIndex<T>());
            }
            return this;
        }

        /// <summary>
        /// Adds a component that has properties set to the EntityBuilder. The component
        /// will be copied and added to the new entity when the EntityBuilder's build method
        /// is called
        /// </summary>
        /// <param name="comp">A copyable component that will be cloned and added to the new entity at build time</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public EntityBuilder With(ICopyableComponent comp)
        {
            bool added = _AddTypeIndice(comp.GetType());

            if (added)
            {
                _componentTemplates.Add(comp);
            }
            return this;
        }

        /// <summary>
        /// Removes the component with type <typeparamref name="T"/> from the EntityBuilder
        /// </summary>
        /// <typeparam name="T">Component type that implements IComponent</typeparam>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public EntityBuilder Without<T>() where T : IComponent
        {
            return Without(typeof(T));
        }

        /// <summary>
        /// Removes the component with the given type from the EntityBuilder 
        /// </summary>
        /// <param name="t">The component type to remove</param>
        /// <returns>Returns itself so that method chaining is possible</returns>
        public EntityBuilder Without(Type t)
        {
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

        /// <summary>
        /// Builds a new entity with the specified components. At this
        /// moment, all the required operations on the added components, component types, creation functions will
        /// be applied and added to the new entity.
        /// </summary>
        /// <param name="scene">The scene to add the new entity to</param>
        /// <returns>The new entity</returns>
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

        /// <summary>
        /// Clones this EntityBuilder
        /// </summary>
        /// <returns>A new cloned EntityBuilder</returns>
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
