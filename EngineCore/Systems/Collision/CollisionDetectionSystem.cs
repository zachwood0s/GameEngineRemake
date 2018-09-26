﻿using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems.Interfaces;
using EngineCore.Components;
using EngineCore.Components.CollisionDetection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineCore.Systems.Collision
{
    public class CollisionDetectionSystem : IExecuteSystem, IInitializeSystem
    {
        private Scene _scene;
        private Group _group;
        private int _id;

        public CollisionDetectionSystem(Scene s)
        {
            _scene = s;
            _group = _scene.GetGroup(GetMatcher());
        }
        public Matcher GetMatcher()
        {
            return new Matcher().AllOf(typeof(Transform2DComponent), typeof(CollisionBoundsComponent));
        }
        public void Execute()
        {
            _group.UpdateAllEntitiesInGroup((Entity entity1, CollisionBoundsComponent collisionComponent1) =>
            {
                Transform2DComponent transform2D1 = entity1.GetComponent<Transform2DComponent>();
                _group.UpdateAllEntitiesInGroup((Entity entity2, CollisionBoundsComponent collisionComponent2) =>
                {
                    Transform2DComponent transform2D2 = entity2.GetComponent<Transform2DComponent>();
                    if (collisionComponent1.ID != collisionComponent2.ID)
                    {                      
                        foreach (CollisionGroup collisionGroup1 in collisionComponent1.CollisionGroups)
                        {
                            foreach (CollisionGroup collisionGroup2 in collisionComponent2.CollisionGroups)
                            {
                                _IsColliding(collisionGroup1, collisionGroup2, transform2D1, transform2D2);
                            }
                        }
                    }
                });
            });
            _UpdatePoints(_group);
        }
        public void Initialize()
        {
            foreach (Entity e in _group)
            {
                e.UpdateComponent<CollisionBoundsComponent>((collisionComponent) =>
                {
                    collisionComponent.ID = _id;
                    Transform2DComponent transform2D = e.GetComponent<Transform2DComponent>();                   
                    foreach (Polygon polygon in collisionComponent.CollisionGroups.SelectMany(x => x.Colliders))
                    {
                        for(int i = 0; i < polygon.Points.Count; i++)
                        {
                            int eIndex = i + 1;
                            if ((i + 1) >= polygon.Points.Count) eIndex = 0;
                            float ex = polygon.Points[eIndex].X - polygon.Points[i].X;
                            float ey = polygon.Points[eIndex].Y - polygon.Points[i].Y;
                            polygon.Edges.Add(new Vector2(ex, ey));
                            polygon.AbsolutePoints.Add(new Vector2(polygon.Points[i].X + transform2D.Position.X, 
                                                                   polygon.Points[i].Y + transform2D.Position.Y));
                        }
                    }
                });
                _id++;
            }
            _UpdatePoints(_group);
        }

        private bool _IsColliding(CollisionGroup cg1, CollisionGroup cg2, Transform2DComponent t1, Transform2DComponent t2)
        {
            bool colliding = true;
            foreach (Polygon p1 in cg1.Colliders)
            {
                foreach (Polygon p2 in cg2.Colliders)
                {
                    colliding = true;
                    Vector2 edge;
                    int c1 = p1.Edges.Count; int c2 = p2.Edges.Count;
                    for (int i = 0; i < c1 + c2; i++)
                    {
                        if (i < c1) edge = p1.Edges[i];
                        else edge = p2.Edges[i - c1];

                        Vector2 axis = new Vector2(-edge.Y, edge.X);
                        axis.Normalize();
                        float min1 = 0; float min2 = 0; float max1 = 0; float max2 = 0;
                        _Project(axis, p1, out min1, out max1);
                        _Project(axis, p2, out min2, out max2);
                        if (_Distance(min1, max1, min2, max2) > 0){ colliding = false; break; }
                    }                
                }
            }
            cg1.Colliding = cg2.Colliding = colliding;
            return colliding;
        }
        private void _Project(Vector2 axis, Polygon p, out float min , out float max)
        {
            float dp = Vector2.Dot(axis, p.AbsolutePoints[0]);
            min = max = dp;
            foreach(Vector2 point in p.AbsolutePoints)
            {
                dp = Vector2.Dot(point, axis);
                if(dp < min) min = dp; 
                else if (dp > max) max = dp;
            }
        }
        private float _Distance(float min1, float max1, float min2, float max2)
        {
            if (min1 < min2) return min2 - max1;
            else return min1 - max2;
        }    
        private void _UpdatePoints(Group group)
        {
            _group.UpdateAllEntitiesInGroup((Entity entity1, CollisionBoundsComponent collisionBounds) =>
            {
                Transform2DComponent transform2D = entity1.GetComponent<Transform2DComponent>();
                foreach (CollisionGroup collisionGroup in collisionBounds.CollisionGroups)
                {
                    foreach (Polygon polygon in collisionGroup.Colliders)
                    {
                        // Update Points
                        List<Vector2> rotPoints = new List<Vector2>();
                        for (int i = 0; i < polygon.AbsolutePoints.Count; i++)
                        {
                            double newAngle = ((Math.PI * transform2D.Rotation) / 180);
                            Matrix matrix = Matrix.Multiply(Matrix.CreateRotationZ((float)newAngle),
                                                            new Matrix(new Vector4(polygon.Points[i].X, 0, 0, 0),
                                                                       new Vector4(polygon.Points[i].Y, 0, 0, 0),
                                                                       Vector4.Zero, Vector4.Zero));
                            rotPoints.Add(new Vector2(matrix.M11, matrix.M21));
                            polygon.AbsolutePoints[i] = new Vector2(matrix.M11, matrix.M21) + transform2D.Position;
                        }
                        // Update edges
                        for (int i = 0; i < polygon.Edges.Count; i++) {
                            int eIndex = i + 1;
                            if ((i + 1) >= polygon.Edges.Count) eIndex = 0;
                            float ex = rotPoints[eIndex].X - rotPoints[i].X;
                            float ey = rotPoints[eIndex].Y - rotPoints[i].Y;
                            polygon.Edges[i] = new Vector2(ex, ey);
                        }
                    }
                }
            });
        }
    }
}
