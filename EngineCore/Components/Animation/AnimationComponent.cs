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
        #region Component Attributes

        public string Name { get; set; }
        public float AnimationTime { get; set; } = 1;
        public string EventAxis { get; set; } = "";
        public string AxisType { get; set; } = "positive";
        public bool Override { get; set; } = true;
        public bool Loop { get; set; } = true;
        public bool CompleteAnimation { get; set; } = true;

        #endregion

        #region Globally Defined Attributes

        public string FileType { get; set; }
        public string FileLocation { get; set; }       
        public float FrameCount { get; set; }

        // Sprite Sheet Variables / Read in from component
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;

        #endregion

        #region Non Readin Attributes

        public int CurrentFrame { get; set; } = 0;
        public DateTime startTime { get; set; } = new DateTime();
        public bool Add { get; set; } = false;
        public bool Remove { get; set; } = false;
        public bool ResetCurrentFrame { get; set; } = true;
        public Rectangle[] Rectangles { get; set; }
        public List<Texture2D> Textures { get; set; } = new List<Texture2D>();

        #endregion

        #region Scripting Methods

        public void Play()
        {
            Add = true;
            ResetCurrentFrame = true;
        }
        public void Stop()
        {
            Remove = true;
            ResetCurrentFrame = true;
        }
        public void Pause()
        {
            ResetCurrentFrame = false;
        }

        #endregion
    }
}
