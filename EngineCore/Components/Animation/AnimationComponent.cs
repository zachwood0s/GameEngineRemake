using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.Animation
{
    [Component]
    public class AnimationComponent: ICopyableComponent
    {
        public AnimationObject[] Animations { get; set; }
        public string FileType { get; set; }
        public string FileLocation { get; set; }
        public List<Texture2D> Textures { get; set; } 

        public IComponent Copy()
        {
            return new AnimationComponent { Animations = Animations, FileType = FileType, FileLocation = FileLocation, Textures = Textures };
        }
    }
}
