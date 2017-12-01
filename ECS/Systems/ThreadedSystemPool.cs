using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ECS.Systems
{
    public class ThreadedSystemPool: SystemPool
    {
        private Thread _thread;

        private int _targetFps;
        private long _currentFps;
        private long _frameCount;
        private DateTime _lastTime;
        private double _avgFps = 0;

        private double _updateTime;
        private bool _noFpsLimit;
        private Stopwatch _stopWatch;

        private bool _isRunning = false;

        public long CurrentFps => _currentFps;
        public bool IsRunning => _isRunning;
        public double AvgFps => _avgFps;

        public ThreadedSystemPool(int fps)
        {
            _targetFps = fps;
            _updateTime = 1000 / _targetFps;
            _thread = new Thread(_ThreadUpdate);
            _stopWatch = new Stopwatch();
        }
        public ThreadedSystemPool()
        {
            _thread = new Thread(_ThreadUpdate);
            _noFpsLimit = true;
            _stopWatch = new Stopwatch();
        }
        public override void Execute()
        {
            _isRunning = true;
            _thread.Start();
            _lastTime = DateTime.Now;
        }

        private void _ThreadUpdate()
        {
            while (_isRunning)
            {
                _frameCount++;
                if((DateTime.Now - _lastTime).TotalSeconds >= 1)
                {
                    _currentFps = _frameCount;
                    _avgFps += _currentFps;
                    _avgFps /= 2;

                    _frameCount = 0;
                    _lastTime = DateTime.Now;
                }

                _stopWatch.Restart();
                base.Execute();
                _stopWatch.Stop();

                if (!_noFpsLimit)
                {
                    long millis = _stopWatch.ElapsedMilliseconds;
                    int waitTime = (int)(_updateTime - millis);
                    if (waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                    }
                }
            }
        }

        public override void CleanUp()
        {
            base.CleanUp();
            _isRunning = false;
        }
    }
}
