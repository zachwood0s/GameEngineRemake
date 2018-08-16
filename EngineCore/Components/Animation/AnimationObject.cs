using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EngineCore.Components.Animation
{
    public class AnimationObject
    {
        public string Name { get; set; }
        public float Speed { get; set; } = 1;
        public int FrameCount { get; set; }
        public int CurrentFrame { get; set; }

        // Sprite Sheet Variables
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;      
    }
}
