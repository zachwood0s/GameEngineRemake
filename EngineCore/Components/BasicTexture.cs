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
    public class BasicTexture: IComponent
    {
        public string FileName { get; private set; }
        public Texture2D Texture { get; private set; }
        
        public BasicTexture(string file)
        {
            FileName = file;
        }

        public BasicTexture(Texture2D tex)
        {
            Texture = tex;
        }
    }
}
