using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ECS.Systems
{
    /// <summary>
    /// Essentially the same as a regular <see cref="SystemPool"/> but
    /// it runs on it's own thread. It can have a target fps or can run
    /// unmanaged.
    /// </summary>
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
        private readonly GameTime _gameTime = new GameTime();
        private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);
        private int _updateFrameLag;

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
            _thread = new Thread(_ThreadUpdate)
            {
                Name = poolName
            };
        }
        public ThreadedSystemPool(string poolName):base(poolName)
        {
            _thread = new Thread(_ThreadUpdate)
            {
                Name = poolName
            };
            _noFpsLimit = true;
        }

        /// <summary>
        /// Starts the thread and executes all the systems contained
        /// in the pool
        /// </summary>
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

                if(_accumulatedElapsedTime > _maxElapsedTime)
                {
                    _accumulatedElapsedTime = _maxElapsedTime;
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
                    _gameTime.ElapsedGameTime = _targetElapsedTime;
                    var stepCount = 0;
                    while(_accumulatedElapsedTime >= _targetElapsedTime)
                    {
                        _gameTime.TotalGameTime += _targetElapsedTime;
                        _accumulatedElapsedTime -= _targetElapsedTime;
                        ++stepCount; 

                        base.Execute();
                    }

                    _updateFrameLag += Math.Max(0, stepCount - 1);

                    if (_gameTime.IsRunningSlowly)
                    {
                        if(_updateFrameLag == 0)
                        {
                            _gameTime.IsRunningSlowly = false;
                        }
                    }
                    else if(_updateFrameLag >= 5)
                    {
                        _gameTime.IsRunningSlowly = true;
                    }

                    if(stepCount == 1 && _updateFrameLag > 0)
                    {
                        _updateFrameLag--;
                    }

                    _gameTime.ElapsedGameTime = TimeSpan.FromTicks(_targetElapsedTime.Ticks * stepCount);
                }
                else
                {
                    _gameTime.ElapsedGameTime = _accumulatedElapsedTime;
                    _gameTime.TotalGameTime += _accumulatedElapsedTime;
                    _accumulatedElapsedTime = TimeSpan.Zero;
                    base.Execute();
                }


            }

        }

        /// <summary>
        /// Stops the system pool and it's thread
        /// </summary>
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
