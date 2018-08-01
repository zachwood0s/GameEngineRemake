using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.InputManager
{
    public class Axis
    {
        public string Name { get; set; }
        public Keys PositiveButton { get; set; }
        public Keys AltPositiveButton { get; set; }
        public Keys NegativeButton { get; set; }
        public Keys AltNegativeButton { get; set; }
    }
}
