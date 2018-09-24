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
            entity.UpdateComponent<EaseComponent>(ease =>
            {
                if(!ease.HasStarted)
                {
                    var startEndDif = ease.EndValue - ease.StartValue;
                    ease.EaseStep = (startEndDif / ease.EaseLength / startEndDif);
                    ease.HasStarted = true;
                    ease.CurrentStep = 0;
                }
                double easeVal = Easings.Interpolate(ease.CurrentStep, ease.EasingFunction);
                ease.SetFunction(easeVal + ease.StartValue);
                ease.CurrentStep += ease.EaseStep;

                if(ease.CurrentStep > 1)
                {
                    entity.Remove<EaseComponent>();
                }
            });
        }
    }
}
