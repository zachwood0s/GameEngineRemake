using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ECS.Systems
{
    class ThreadedSystemPool: SystemPool
    {
        private Thread _thread;
        private int _fps;

        public ThreadedSystemPool(int fps)
        {
            _fps = fps;
            _thread = new Thread(Execute);
        }
        public override void Execute()
        {
            base.Execute();

            Execute();
        }
        public override void Initialize()
        {
            base.Initialize();

            _thread.Start();
        }
    }
}
