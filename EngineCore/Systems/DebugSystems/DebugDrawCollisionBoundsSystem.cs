using C3.XNA;
using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using EngineCore.Components;
using EngineCore.Components.CollisionDetection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace EngineCore.Systems.DebugSystems
{
    public class DebugDrawCollisionBoundsSystem : GroupExecuteSystem
    {
        public static Color DrawColorNeg { get; set; } = Color.Green;
        public static Color DrawColorPos { get; set; } = Color.Red;

        private Func<bool> _getDebugState;
        private SpriteBatch _spriteBatch;

        public DebugDrawCollisionBoundsSystem(Scene scene, SpriteBatch spriteBatch, Func<bool> getDebugState) : base(scene)
        {
            _getDebugState = getDebugState;
            _spriteBatch = spriteBatch;
        }
        public override void Execute(Entity entity)
        {
            if (_getDebugState())
            {
                entity.UpdateComponents<Transform2DComponent, CollisionBoundsComponent>((transform, bounds) =>
                {
                    foreach (CollisionGroup collisionGroup in bounds.CollisionGroups)
                    {
                        foreach (Polygon polygon in collisionGroup.Colliders)
                        {
                            for (int i = 0; i < polygon.AbsolutePoints.Count; i++)
                            {
                                int x = i + 1;
                                if (x >= polygon.AbsolutePoints.Count) x = 0;
                                if (collisionGroup.Colliding)
                                {
                                    _spriteBatch.DrawLine(polygon.AbsolutePoints[i], polygon.AbsolutePoints[x], DrawColorPos, 2);
                                }
                                else
                                {
                                    _spriteBatch.DrawLine(polygon.AbsolutePoints[i], polygon.AbsolutePoints[x], DrawColorNeg, 2);
                                }
                            }
                        }
                    }
                });
            }
        }
        public override Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(CollisionBoundsComponent));
        }
    }
}
