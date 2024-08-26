using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame_Base.Project.Utility.Basics
{
    public class Timer
    {
        public event EventHandler<EventArgs> OnTimerFinished;

        private bool _active = true;
        private bool _loop;

        private float _currentTime;
        private float _timerLength;

        public Timer(float pTimerLength, bool pLoop = true)
        {
            _timerLength = pTimerLength;
            _loop = pLoop;
        }

        public void Update()
        {
            UpdateTimer();
        }

        public void SetTimerLength(float pTimerLength)
        {
            _timerLength = pTimerLength;
        }

        public void RestartTimer()
        {
            if(!_active) _active = true;
            _currentTime = 0;
        }

        public void StopTimer()
        {
            if (_active) _active = false;
        }

        public void ContinueTimer()
        {
            if (!_active) _active = true;
        }

        private void UpdateTimer()
        {
            if (_active)
            {
                _currentTime += Globals.TotalSeconds;

                if (_currentTime >= _timerLength)
                {
                    OnTimerFinished?.Invoke(this, new EventArgs());

                    if (_loop) _currentTime = 0;
                    else _active = false;
                }
            }
        }
    }
}
