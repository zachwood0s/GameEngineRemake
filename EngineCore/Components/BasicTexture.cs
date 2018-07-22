using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    [Component]
    public class BasicTexture: ICopyableComponent
    {
        public string FileName { get; set; }
        public Texture2D Texture { get; set; }
        
        public BasicTexture(string file)
        {
            FileName = file;
        }

        public BasicTexture(Texture2D tex)
        {
            Texture = tex;
        }

        public IComponent Copy()
        {
            return new BasicTexture(FileName) { Texture = Texture };
        }
    }
}
