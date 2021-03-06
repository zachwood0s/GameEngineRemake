﻿using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    [Component]
    public class Transform2DComponent : IComponentHasDefault, ICopyableComponent
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Transform2DComponent() { }

        public Transform2DComponent(int x, int y, float r)
        {
            Position = new Vector2(x, y);
            Rotation = r;
        }
        public Transform2DComponent(int x, int y)
        {
            Position = new Vector2(x, y);
            Rotation = 0;
        }
        public void SetDefaults()
        {
            Position = new Vector2(0, 0);
            Rotation = 0;
        }

        public IComponent Copy()
        {
            return new Transform2DComponent() { Position = Position, Rotation = Rotation };
        }
    }
}
