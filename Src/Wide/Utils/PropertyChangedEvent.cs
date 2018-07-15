using System;
using System.ComponentModel;

namespace Wide.Utils
{
    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual Object OldValue { get; private set; }
        public virtual Object NewValue { get; private set; }
        public virtual String Description { get; private set; }

        public PropertyChangedExtendedEventArgs(String propertyName, Object oldValue, Object newValue, String description) 
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Description = description;
        }
    }
}
