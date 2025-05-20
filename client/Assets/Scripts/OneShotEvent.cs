using JetBrains.Annotations;
using SpacetimeDB.Types;
using Util;

public interface IHasEntity
{
    public Entity Entity { get; }
}

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
            if (_invoked)
            {
                return;
            }
            OnInvoked?.Invoke();
            _invoked = true;
        }
    }
}