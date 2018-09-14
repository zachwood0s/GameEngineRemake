using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.Scripting
{
    public abstract class ScriptBaseComponent<T>: ICopyableComponent
    {
        public string ScriptFile { get; set; }
        public string FunctionName { get; set; }
        public T ScriptAction { get; set; }

        public IComponent Copy()
        {
            var copy = CopyInstantiator();
            copy.ScriptFile = ScriptFile;
            copy.FunctionName = FunctionName;
            copy.ScriptAction = ScriptAction;
            return copy;
        }

        /// <summary>
        /// We need this function so that when the JSON loader loads and 
        /// makes a copy of the object or when used in an entity builder
        /// it creates the correct instance and not a ScriptBaseComponent.
        /// </summary>
        /// <returns></returns>
        protected abstract ScriptBaseComponent<T> CopyInstantiator(); 
    }
}
