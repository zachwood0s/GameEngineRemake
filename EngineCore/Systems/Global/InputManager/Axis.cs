using Microsoft.Xna.Framework;
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
        public PlayerIndex PlayerIndex { get; set; }
        public Keys PositiveKeyButton { get; set; }
        public Keys AltPositiveKeyButton { get; set; }
        public Keys NegativeKeyButton { get; set; }
        public Keys AltNegativeKeyButton { get; set; }
        public string PositiveMouseButton { get; set; }
        public string AltPositiveMouseButton { get; set; }
        public string NegativeMouseButton { get; set; }
        public string AltNegativeMouseButton { get; set; }
        public Buttons PositiveGamePadButton { get; set; }
        public Buttons AltPositiveGamePadButton { get; set; }
        public Buttons NegativeGamePadButton { get; set; }
        public Buttons AltNegativeGamePadButton { get; set; }
    }
}
