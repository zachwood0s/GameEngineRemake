using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework;
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
        public List<AnimationObject> Animations { get; set; }
        public List<AnimationObject> CurrentAnimations { get; set; } = new List<AnimationObject>();

        public IComponent Copy()
        {
            return new AnimationComponent { Animations = Animations };
        }
    }
    public class AnimationObject
    {
        // Read in from the component
        public string Name { get; set; }
        public float AnimationTime { get; set; } = 1;
        public string EventAxis { get; set; }
        public string AxisType { get; set; } = "positive";
        public bool Override { get; set; } = true;

        // Read in globally
        public string FileType { get; set; }
        public string FileLocation { get; set; }       
        public float FrameCount { get; set; }

        // Sprite Sheet Variables / Read in from component
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;

        // Non read in variables
        public int CurrentFrame { get; set; } = 0;
        public DateTime startTime { get; set; } = new DateTime();
        public bool Run { get; set; } = false;
        public bool ResetFrame { get; set; } = true;
        public Rectangle[] Rectangles { get; set; }
        public List<Texture2D> Textures { get; set; } = new List<Texture2D>();

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
