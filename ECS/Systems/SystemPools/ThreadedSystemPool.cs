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
        private long _frameCount;
        private DateTime _lastFrameReadTime;

        private TimeSpan _targetElapsedTime;
        private bool _noFpsLimit;
        private Stopwatch _stopwatch;
        private long _previousTicks = 0;
        private TimeSpan _accumulatedElapsedTime;

        private bool _isRunning = false;

        public bool IsRunning => _isRunning;

        public int TargetFPS
        {
            get => _targetFps;
            set
            {
                if (value > 0) _SetTargetFPS(value);
            }
        }

        public ThreadedSystemPool(string poolName, int fps):base(poolName)
        {
            _SetTargetFPS(fps);
            _thread = new Thread(_ThreadUpdate);
        }
        public ThreadedSystemPool(string poolName):base(poolName)
        {
            _thread = new Thread(_ThreadUpdate);
            _noFpsLimit = true;
        }
        public override void Execute()
        {
            if (!_isRunning)
            {
                _ResetTiming();
                _thread.Start();
            }
        }
        protected void _ResetTiming()
        {
            _isRunning = true;
            _lastFrameReadTime = DateTime.Now;
            _stopwatch = Stopwatch.StartNew();
        }

        protected void _ThreadUpdate()
        {
           
            while (_isRunning)
            {
                //Pulled this from Monogame Game class's tick function
                //The goto is a little wierd but I guess it works?
                //Wouldn't a while loop work just as good
                RetryTick:

                    var currentTicks = _stopwatch.Elapsed.Ticks;
                    _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
                    _previousTicks = currentTicks;

                    if(!_noFpsLimit && _accumulatedElapsedTime < _targetElapsedTime)
                    {
                        var sleepTime = (int)(_targetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;

                        Thread.Sleep(sleepTime);

                        goto RetryTick;
                    }


                    _frameCount++;
                    if((DateTime.Now - _lastFrameReadTime).TotalSeconds >= 1)
                    {
                        _currentFps = _frameCount;
                        _avgFps += _currentFps;
                        _avgFps /= 2;

                        _frameCount = 0;
                        _lastFrameReadTime = DateTime.Now;
                    }

                if (!_noFpsLimit)
                {
                    var stepCount = 0;
                    while(_accumulatedElapsedTime >= _targetElapsedTime)
                    {
                        _accumulatedElapsedTime -= _targetElapsedTime;
                        ++stepCount; 

                        base.Execute();
                    }
                }
                else
                {
                    _accumulatedElapsedTime = TimeSpan.Zero;
                }


            }

        }

        public override void CleanUp()
        {
            base.CleanUp();
            _isRunning = false;
            _stopwatch?.Stop();
        }

        private void _SetTargetFPS(int fps)
        {
            _targetFps = fps;
            _targetElapsedTime = TimeSpan.FromMilliseconds(1000 / _targetFps);
        }
    }
}
