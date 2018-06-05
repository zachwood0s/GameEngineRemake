using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// These are not thread safe i don't think and I'm not sure if they can be.
/// </summary>
namespace ECS.Components
{
    /*
    public delegate void ComponentPropertySet(int componentTypeIndex, IComponent component);
    public class ComponentProperty<T>
    {
        private Action _changedAction;
        public ComponentProperty(Action changedAction) {
            _changedAction = changedAction;
        }

        private T _value;
        public T Value
        {
            get => _value; 
            set
            {
                _value = value;
                _changedAction();
            }
        }
    }
    I dont think these are a good idea

    public class SettableComponent : IComponent
    {
        private event ComponentPropertySet _OnComponentSet;
        private bool _isComponentIndexCached = false;
        private int _cachedComponentIndex;

        protected void _HandleComponentPropertySet()
        {
            if (!_isComponentIndexCached)
            {
                _cachedComponentIndex = ComponentPool.GetComponentIndex(GetType());
                _isComponentIndexCached = true;
            }
            _OnComponentSet?.Invoke(_cachedComponentIndex, this);
        }

        public void SubscribeToChanges(ComponentPropertySet handler)
        {
            _OnComponentSet += handler;
        }
        public void UnSubscribeToChanges(ComponentPropertySet handler)
        {
            _OnComponentSet -= handler;
        }
    }
    */
}
