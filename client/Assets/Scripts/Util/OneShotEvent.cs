using System;

namespace Util
{
    public class OneShotEvent
    {
        private bool _invoked;
        public delegate void OneShotEventHandler();
        public event OneShotEventHandler OnInvoked;

        public void Subscribe(OneShotEventHandler handler)
        {
            if (_invoked)
            {
                handler();
            }
            else
            {
                OnInvoked += handler;
            }
        }

        public void Invoke()
        {
            OnInvoked?.Invoke();
            _invoked = true;
        }
    }
}