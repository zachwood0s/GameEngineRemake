using ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using EngineCore.Components;
using ECS.Matching;
using ECS.Entities;

namespace EngineCore.Systems
{
    public class TestMovementSystem : ThreadSafeReactiveSystem
    {
        public TestMovementSystem(Scene s): base(s) { }
        public override void Execute(Entity entity)
        {
            entity.UpdateComponent((Transform2DComponent comp) =>
            {
                comp.Position = new Microsoft.Xna.Framework.Vector2(comp.Position.X + .1f, comp.Position.Y);
            });

            //Transform2DComponent component = entity.GetComponent<Transform2DComponent>();
            //entity.UpdateComponent(new Transform2DComponent((int)component.Position.X + 1, (int)component.Position.Y));
        }

        protected override Matcher GetMatcher()
        {
            return new Matcher().Of<Transform2DComponent>();
        }
    }
}
