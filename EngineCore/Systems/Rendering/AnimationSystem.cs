using ECS;
using ECS.Systems;
using ECS.Systems.Interfaces;
using EngineCore.Components;
using EngineCore.Systems.Global.InputManager;
using EngineCore.Components.Animation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;
using ECS.Matching;
using ECS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using EngineCore.Systems.Global.Animation;

namespace EngineCore.Systems.Rendering
{
    public class AnimationSystem : GroupExecuteSystem, IInitializeSystem
    {
        private SpriteBatch _spriteBatch;
        private ContentManager _contentManager;
        private InputManager _inputManager;
        private GlobalAnimationSystem _globalAnimationSystem;

        public AnimationSystem(Scene s, SpriteBatch spriteBatch, ContentManager contentManager, InputManager inputManager, 
                                GlobalAnimationSystem globalAnimationSystem) : base(s)
        {
            _spriteBatch = spriteBatch;
            _contentManager = contentManager;
            _inputManager = inputManager;
            _globalAnimationSystem = globalAnimationSystem;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(AnimationComponent));
        }

        public override void Execute(Entity e)
        {
            e.UpdateComponents((AnimationComponent animationComponent, Transform2DComponent transform2D) =>
            {
                // All animations loop
                foreach (AnimationObject animation in animationComponent.Animations)
                {             
                    animation.Add = _inputManager.GetAxisDown(animation.EventAxis, animation.AxisType);
                    animation.Remove = _inputManager.GetAxisReleased(animation.EventAxis, animation.AxisType);               
                    if (animation.Add)  // Add animations to list of current animations
                    {
                        if (!animationComponent.CurrentAnimations.Any(item => item == animation))
                        {
                            if (animation.Override) animationComponent.CurrentAnimations.Clear();
                            animationComponent.CurrentAnimations.Add(animation);
                        }
                        animation.Add = false;
                    }
                    else if(animation.Remove)   // Remove animations from list of current animations
                    {
                        if (animationComponent.CurrentAnimations.Any(item => item == animation))
                        {
                            animationComponent.CurrentAnimations.Remove(animation);
                        }
                        animation.CurrentFrame = 0;
                        animation.Remove = false;
                    }       
                }

                // Current animations loop
                BasicTextureComponent basicTexture = e.GetComponent<BasicTextureComponent>();
                if (basicTexture != null) basicTexture.Render = true;
                foreach (AnimationObject animation in animationComponent.CurrentAnimations)
                {
                    // Render Texture
                    if (animation.FileType == "SpriteSheet")
                    {
                        _spriteBatch.Draw(animation.Textures[0], transform2D.Position, animation.Rectangles[animation.CurrentFrame], Color.White);
                    }
                    else if (animation.FileType == "TextureFolder" || animation.FileType == "FileList")
                    {
                        // Handle Texture List
                    }
                    if (basicTexture != null) basicTexture.Render = false;

                    // Update the animation frame
                    TimeSpan timeDiff = DateTime.Now - animation.startTime;
                    float frameTime = (animation.AnimationTime / animation.FrameCount) * 1000;
                    if (timeDiff.Milliseconds >= frameTime)
                    {
                        animation.CurrentFrame++;
                        animation.startTime = DateTime.Now;
                        if (animation.CurrentFrame >= animation.FrameCount)
                        {
                            animation.CurrentFrame = 0;
                        }
                    }
                }
            });
        }

        public void Initialize()
        {
            Group entitiesToLoad = Scene.GetGroup(new Matcher().Of<AnimationComponent>());
            foreach(Entity e in entitiesToLoad)
            {
                e.UpdateComponent<AnimationComponent>((animationComponent) =>
                {
                    foreach (AnimationObject animation in animationComponent.Animations)
                    {
                        _setAnimationVars(animation);
                        if (animation.FileType == "TextureFolder")
                        {
                            animation.Textures = _loadTextures(animation);
                        }
                        else if (animation.FileType == "SpriteSheet")
                        {
                            animation.Textures.Add(_contentManager.Load<Texture2D>(animation.FileLocation));
                            _loadSprites(animation);
                        }
                        else if (animation.FileType == "FileList")
                        {
                            // Handle File List
                        }
                        else
                        {
                            Debug.WriteLine($"'{animation.FileType}' is not a valid file type");
                        }
                    }
                });
            }
        }

        #region Private Helper Methods

        private void _loadSprites(AnimationObject animation)
        {
            animation.Rectangles = new Rectangle[Convert.ToInt32(animation.FrameCount)];
            for (int i = 0; i < animation.FrameCount; i++)
            {
                animation.Rectangles[i] = new Rectangle((animation.SpriteSize[0] * i) + animation.SpriteStartSite[0],
                                            animation.SpriteStartSite[1], animation.SpriteSize[0], animation.SpriteSize[1]);
            }
        }
        private List<Texture2D> _loadTextures(AnimationObject animation)
        {
            List<Texture2D> textures = new List<Texture2D>();
            if (Directory.Exists(animation.FileLocation))
            {
                string[] fileEntries = Directory.GetFiles(animation.FileLocation);
                foreach (string file in fileEntries)
                {
                    textures.Add(_contentManager.Load<Texture2D>(animation.FileLocation + "/" + file));
                }
            }
            else
            {
                Debug.WriteLine($"File directory '{animation.FileLocation}' does not exist");
            }
            return textures;
        }
        private void _setAnimationVars(AnimationObject animation)
        {
            foreach(AnimationContainer animationContainer in _globalAnimationSystem.Animations)
            {
                foreach(AnimationData animationData in animationContainer.Animations)
                {
                    if(animation.Name == animationData.Name)
                    {
                        animation.FileType = animationContainer.FileType;
                        animation.FileLocation = animationContainer.FileLocation;
                        animation.FrameCount = animationData.FrameCount;
                        animation.SpriteSize = animationData.SpriteSize;
                        animation.SpriteStartSite = animationData.SpriteStartSite;
                        animation.FileStartNumber = animationData.FileStartNumber;
                    }
                }
            }
        }
    }

    #endregion
}
