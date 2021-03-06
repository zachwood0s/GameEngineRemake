﻿using ECS.Entities;
using ECS.Matching;
using ECS.Systems.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Systems
{
    public abstract class GroupExecuteSystem : IExecuteSystem
    {
        private Scene _scene;
        private Group _group;

        protected Scene Scene => _scene;
        protected Group Group => _group;

        public GroupExecuteSystem(Scene scene)
        {
            _scene = scene;
            _group = _scene.GetGroup(GetMatcher());
        }
        public abstract Matcher GetMatcher();
        public void Execute()
        {
            for(int i = 0; i<_group.EntityCount; i++)
            {
                Execute(_group[i]);
            }
        }
        public abstract void Execute(Entity entity);
    }
}
