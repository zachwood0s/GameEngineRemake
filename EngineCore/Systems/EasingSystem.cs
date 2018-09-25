using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using EngineCore.Components;
using EngineCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems
{
    public class EasingSystem : GroupExecuteSystem
    {
        public EasingSystem(Scene scene) : base(scene)
        {
        }
        public override Matcher GetMatcher()
        {
            return new Matcher().Of<EaseComponent>();
        }
        public override void Execute(Entity entity)
        {
            var shouldRemove = false;
            entity.UpdateComponent<EaseComponent>(ease =>
            {
                if(!ease.HasStarted)
                {
                    ease.HasStarted = true;
                    ease.CurrentStep = 0;
                }
                ease.CurrentStep++;
                var startEndDif = ease.EndValue - ease.StartValue;
                double easeVal = Easings.Interpolate(ease.CurrentStep, ease.StartValue, startEndDif, ease.EaseLength, ease.EasingFunction);
                ease.SetFunction(ease.StartValue + easeVal);

                Console.WriteLine(easeVal);

                if(ease.CurrentStep >= ease.EaseLength)
                {
                    ease.SetFunction(ease.EndValue);
                    shouldRemove = true;
                }
            });
            if (shouldRemove) entity.Remove<EaseComponent>();
        }
    }
}
