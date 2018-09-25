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
        public float StartValue { get; set; }
        public float EndValue { get; set; }
        public float EaseLength { get; set; }
        public float CurrentStep { get; set; }
        public float EaseStep { get; set; }
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
