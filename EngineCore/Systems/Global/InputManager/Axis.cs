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

        #region Non Read in Attributes

        public float CurrentValue { get; set; } = 0;

        #endregion

        #region Global Attributes

        public string Name { get; set; }

        public float axisSpeed { get; set; } = .0000096f;    // Maybe a better name

        #endregion

        #region Keyboard Attributes

        public Keys PositiveKeyButton { get; set; }
        public Keys AltPositiveKeyButton { get; set; }
        public Keys NegativeKeyButton { get; set; }
        public Keys AltNegativeKeyButton { get; set; }

        #endregion

        #region Mouse Attributes

        public string PositiveMouseButton { get; set; }
        public string AltPositiveMouseButton { get; set; }
        public string NegativeMouseButton { get; set; }
        public string AltNegativeMouseButton { get; set; }

        #endregion

        #region GamePad Attributes

        public PlayerIndex[] PlayerIndex { get; set; }
        public Buttons PositiveGamePadButton { get; set; }
        public Buttons AltPositiveGamePadButton { get; set; }
        public Buttons NegativeGamePadButton { get; set; }
        public Buttons AltNegativeGamePadButton { get; set; }
        public string JoystickAxis { get; set; }
        public string AltJoystickAxis { get; set; }

        #endregion
    }
}
