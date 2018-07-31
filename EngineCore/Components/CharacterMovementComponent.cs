using ECS.Attributes;
using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    [Component]
    public class CharacterMovementComponent : ICopyableComponent, IComponentHasDefault
    {
        public float CharacterMovementSpeed { get; set; }
        public string HorizontalInputAxis { get; set; }
        public string VerticalInputAxis { get; set; }
        public IComponent Copy()
        {
            return new CharacterMovementComponent
            {
                CharacterMovementSpeed = CharacterMovementSpeed,
                HorizontalInputAxis = HorizontalInputAxis,
                VerticalInputAxis = VerticalInputAxis
            };
        }

        public void SetDefaults()
        {
            CharacterMovementSpeed = 1;
            HorizontalInputAxis = "horizontal";
            VerticalInputAxis = "vertical";
        }
    }
}
