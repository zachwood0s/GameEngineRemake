using ECS.Systems.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Systems.Rendering
{
    public class SpriteBatchBeginSystem: IExecuteSystem
    {
        private SpriteBatch _spriteBatch;
        public SpriteBatchBeginSystem(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }
        public void Execute()
        {
            _spriteBatch.Begin();
        }
    }
    public class SpriteBatchEndSystem: IExecuteSystem
    {
        private SpriteBatch _spriteBatch;
        public SpriteBatchEndSystem(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }
        public void Execute()
        {
            _spriteBatch.End();
        }
    }
}
