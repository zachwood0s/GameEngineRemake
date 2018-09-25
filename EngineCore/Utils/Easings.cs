using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Utils
{
    using System;

    static public class Easings
    {
        /// <summary>
        /// Constant Pi.
        /// </summary>
        private const double PI = Math.PI;

        /// <summary>
        /// Constant Pi / 2.
        /// </summary>
        private const double HALFPI = Math.PI / 2.0f;

        /// <summary>
        /// Easing Functions enumeration
        /// </summary>
        public enum Functions
        {
            Linear,
            QuadraticEaseIn,
            QuadraticEaseOut,
            QuadraticEaseInOut,
            QuarticEaseIn,
            QuarticEaseOut,
            QuarticEaseInOut,
            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,
        }

        /// <summary>
        /// Interpolate using the specified function.
        /// </summary>
        static public double Interpolate(float p, float b, float c, float d, Functions function)
        {
            switch (function)
            {
                default:
                case Functions.Linear: return _Linear(p, b, c, d);
                case Functions.QuadraticEaseOut: return _EaseOut(p, b, c, d);
                case Functions.QuadraticEaseIn: return _EaseIn(p, b, c, d);
                case Functions.QuadraticEaseInOut: return _EaseInOut(p, b, c, d);
                case Functions.QuarticEaseIn: return _EaseInQuart(p, b, c, d);
                case Functions.QuarticEaseOut: return _EaseOutQuart(p, b, c, d);
                case Functions.QuarticEaseInOut: return _EaseInOutQuart(p, b, c, d);
                case Functions.BackEaseIn: return _EaseInBack(p, b, c, d);
                case Functions.BackEaseOut: return _EaseOutBack(p, b, c, d);
                case Functions.BackEaseInOut: return _EaseInOutBack(p, b, c, d);
            }
        }

        private static float _Linear(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        private static float _EaseIn(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t + b;
        }
        private static float _EaseOut(float t, float b, float c, float d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }
        private static float _EaseInOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t + b;
            else
            {
                t--;
                return -c / 2 * (t * (t - 2) - 1) + b;
            }
        }
        private static float _EaseInQuart(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t * t + b;
        }
        private static float _EaseOutQuart(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return -c * (t * t * t * t - 1) + b;
        }
        private static float _EaseInOutQuart(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t + b;
            else
            {
                t -= 2;
                return -c / 2 * (t * t * t * t - 2) + b;
            }
        }
        private static float _EaseInBack(float t, float b, float c, float d)
        {
            float s = 1.7f;
            t /= d;
            return c * t * t * ((s + 1) * t - s) + b;
        }
        private static float _EaseOutBack(float t, float b, float c, float d)
        {
            float s = 1.7f;
            t /= d;
            t--;
            return c * (t * t * ((s + 1) * t + s) + 1) + b;
        }
        private static float _EaseInOutBack(float t, float b, float c, float d)
        {
            float s = 1.7f;
            s *= 1.5f;
            t /= d / 2;
            if (t < 1) return c / 2 * (t * t * ((s + 1) * t - s)) + b;
            else
            {
                t -= 2;
                return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
            }
        }

    }
}
