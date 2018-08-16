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
        public float AnimationTime { get; set; } = 1;
        public float FrameCount { get; set; }

        // Sprite Sheet Variables
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;

        public int CurrentFrame { get; set; }
        public DateTime startTime { get; set; } = new DateTime();
    }
}
