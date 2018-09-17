﻿using ECS.Attributes;
using ECS.Components;
using ECS.Entities;
using EngineCore.Components.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.UI.Events
{
    [Component]
    public class UIOnClickComponent : ScriptBaseComponent<Action<Entity>>
    {
        public bool WasPressed { get; set; }
        protected override ScriptBaseComponent<Action<Entity>> CopyInstantiator()
        {
            return new UIOnClickComponent();
        }

        public override IComponent Copy()
        {
            var clone = base.Copy() as UIOnClickComponent;
            clone.WasPressed = WasPressed;
            return clone;
        }
    }
}
