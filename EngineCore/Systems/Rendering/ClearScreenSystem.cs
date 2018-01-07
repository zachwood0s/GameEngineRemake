using ECS.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Rendering
{
    public class ClearScreenSystem: IExecuteSystem
    {
        GraphicsDevice _graphics;
        Color _colorToClearTo;
        public ClearScreenSystem(GraphicsDevice g, Color colorToClearTo)
        {
            _graphics = g;
            _colorToClearTo = colorToClearTo;
        }
        public void Execute()
        {
            _graphics.Clear(_colorToClearTo);
        }
    }
}
