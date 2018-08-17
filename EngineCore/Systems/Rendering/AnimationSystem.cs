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

namespace EngineCore.Systems.Rendering
{
    public class AnimationSystem : GroupExecuteSystem, IInitializeSystem
    {
        private SpriteBatch _spriteBatch;
        private ContentManager _contentManager;
        private InputManager _inputManager;

        public AnimationSystem(Scene s, SpriteBatch spriteBatch, ContentManager contentManager, InputManager inputManager) : base(s)
        {
            _spriteBatch = spriteBatch;
            _contentManager = contentManager;
            _inputManager = inputManager;
        }

        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(AnimationComponent));
        }

        public override void Execute(Entity e)
        {
            e.UpdateComponents((AnimationComponent animationComponent, Transform2DComponent transform2D) =>
            {
                foreach (AnimationObject animation in animationComponent.Animations)
                {
                    // Render Texture
                    if (animationComponent.FileType == "SpriteSheet")
                    {
                        Rectangle source = new Rectangle((animation.SpriteSize[0] * animation.CurrentFrame) +
                                                            animation.SpriteStartSite[0], animation.SpriteStartSite[1],
                                                            animation.SpriteSize[0], animation.SpriteSize[1]);
                        _spriteBatch.Draw(animationComponent.Textures[0], transform2D.Position, source, Color.White);

                    }
                    else if (animationComponent.FileType == "TextureFolder" || animationComponent.FileType == "FileList")
                    {
                        // Handle Texture List
                    }

                    // Update the animation frame
                    if (animation.Run)
                    {                      
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
                    else if(animation.ResetFrame)
                    {
                        animation.CurrentFrame = 0;
                    }

                    // Predefine animation events handeled
                    if (animation.AxisType == "positive" )
                    {
                        animation.Run = _inputManager.GetPositiveAxisPressed(animation.EventAxis);
                    }
                    else
                    {
                        animation.Run = _inputManager.GetPositiveAxisPressed(animation.EventAxis);
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
                    if (animationComponent.FileType == "TextureFolder")
                    {
                        animationComponent.Textures = _getTextures(animationComponent);
                    }
                    else if (animationComponent.FileType == "SpriteSheet")
                    {
                        animationComponent.Textures.Add(_contentManager.Load<Texture2D>(animationComponent.FileLocation));
                    }
                    else if (animationComponent.FileType == "FileList")
                    {
                        // Handle File List
                    }
                    else
                    {
                        Debug.WriteLine($"'{animationComponent.FileType}' is not a valid file type");
                    }
                });
            }
        }

        private List<Texture2D> _getTextures(AnimationComponent animation)
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
    }
}
