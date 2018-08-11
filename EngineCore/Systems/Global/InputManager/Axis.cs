using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Global.InputManager
{
    public enum MouseButtons { Left, Right, Middle, None }
    public enum Joystick { LeftX, RightX, LeftY, RightY, None }
    public class Axis
    {
        #region Global Attributes

        public string Name { get; set; }
        public bool Invert { get; set; } = false;

        #endregion

        #region Keyboard Attributes

        public Keys PositiveKeyButton { get; set; }
        public Keys AltPositiveKeyButton { get; set; }
        public Keys NegativeKeyButton { get; set; }
        public Keys AltNegativeKeyButton { get; set; }

        #endregion

        #region Mouse Attributes

        public MouseButtons PositiveMouseButton { get; set; } = MouseButtons.None;
        public MouseButtons AltPositiveMouseButton { get; set; } = MouseButtons.None;
        public MouseButtons NegativeMouseButton { get; set; } = MouseButtons.None;
        public MouseButtons AltNegativeMouseButton { get; set; } = MouseButtons.None;

        #endregion

        #region GamePad Attributes

        public PlayerIndex[] PlayerIndex { get; set; }
        public Buttons PositiveGamePadButton { get; set; }
        public Buttons AltPositiveGamePadButton { get; set; }
        public Buttons NegativeGamePadButton { get; set; }
        public Buttons AltNegativeGamePadButton { get; set; }
        public Joystick JoystickAxis { get; set; } = Joystick.None;
        public Joystick AltJoystickAxis { get; set; } = Joystick.None;

        #endregion
    }
}
