using ECS.Systems.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.Animation
{
    public class GlobalAnimationSystem: IInitializeSystem
    {
        public string InputFile { get; set; }
        public List<AnimationContainer> Animations { get; private set; }

        public void Initialize()
        {
            Animations = JsonConvert.DeserializeObject<List<AnimationContainer>>(File.ReadAllText("./" + InputFile));
        }
    }
    public class AnimationData
    {
        public string Name { get; set; }
        public float FrameCount { get; set; }

        // Sprite Sheet Variables
        public int[] SpriteSize { get; set; }
        public int[] SpriteStartSite { get; set; } = { 0, 0 };

        // File Folder / File List Variables
        public int FileStartNumber { get; set; } = 0;
        public int FileEndNumber { get; set; } = -1;
    }
    public class AnimationContainer
    {
        public AnimationData[] Animations { get; set; }
        public string FileType { get; set; }
        public string FileLocation { get; set; }
    }
}
