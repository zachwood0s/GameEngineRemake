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
        public string EventAxis { get; set; }
        public string AxisType { get; set; } = "positive";

        // Sprite Sheet Variables
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;

        // Non read in variables
        public int CurrentFrame { get; set; } = 0;
        public DateTime startTime { get; set; } = new DateTime();
        public bool Run { get; set; } = false;
        public bool ResetFrame { get; set; } = true;

        public void Play()
        {
            Run = true;
            ResetFrame = true;
        }
        public void Stop()
        {
            Run = false;
            CurrentFrame = 0;
            ResetFrame = true;
        }
        public void Pause()
        {
            Run = false;
            ResetFrame = false;
        }
    }
}
