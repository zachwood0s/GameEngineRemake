using ECS.Attributes;
using ECS.Components;
using EngineCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    [Component]
    public class EaseComponent:ICopyableComponent
    {
        public Action<double> SetFunction { get; set; }
        public Easings.Functions EasingFunction { get; set; }
        public double StartValue { get; set; }
        public double EndValue { get; set; }
        public double EaseLength { get; set; }
        public double CurrentStep { get; set; }
        public double EaseStep { get; set; }
        public bool HasStarted { get; set; } = false;

        public IComponent Copy()
        {
            return new EaseComponent()
            {
                SetFunction = SetFunction,
                EasingFunction = EasingFunction,
                StartValue = StartValue,
                EndValue = EndValue,
                EaseLength = EaseLength,
                CurrentStep = CurrentStep,
                EaseStep = EaseStep
            };
        }
    }
}
