using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.Scripting
{
    public class ScriptBaseComponent<T>: ICopyableComponent
    {
        public string ScriptFile { get; set; }
        public string FunctionName { get; set; }
        public T ScriptAction { get; set; }

        public IComponent Copy()
        {
            return new ScriptBaseComponent<T>()
            {
                ScriptFile = ScriptFile,
                FunctionName = FunctionName,
                ScriptAction = ScriptAction
            };
        }
    }
}
