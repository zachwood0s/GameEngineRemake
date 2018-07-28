using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    [Component]
    public class BasicTextureComponent: ICopyableComponent
    {
        public string FileName { get; set; }
        public Texture2D Texture { get; set; }
        
        public IComponent Copy()
        {
            return new BasicTextureComponent { Texture = Texture, FileName = FileName };
        }
    }
}
