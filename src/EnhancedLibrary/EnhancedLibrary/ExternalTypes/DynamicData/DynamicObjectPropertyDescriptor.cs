using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace EnhancedLibrary.ExternalTypes.DynamicData
{
    class DynamicObjectPropertyDescriptor<T> : PropertyDescriptor
        where T : IDynamicObject
    {
        readonly bool m_shouldSerializeValue;
        readonly T m_item;

        public DynamicObjectPropertyDescriptor(T item, string name) : base(name, new Attribute[0])
        {
            m_item = item;
        }

        public DynamicObjectPropertyDescriptor(T item, string name, bool shouldSerializeValue) : this(item, name)
        {
            m_shouldSerializeValue = shouldSerializeValue;
        }



        T Get(object component)
        {
            return (T)component;
        }



        public override bool IsReadOnly                                  { get { return false; } }
        public override bool CanResetValue(object component)             { return true; }
        public override void ResetValue(object component)                { Get(component)[Name] = null; }

        public override Type ComponentType                               { get { return typeof(T); } }

        public override object  GetValue(object component)               { return Get(component)[Name]; }
        public override void    SetValue(object component, object value) { Get(component)[Name] = value; }

        public override bool ShouldSerializeValue(object component)      { return m_shouldSerializeValue; }

        public override Type PropertyType { 
            
            get {
                if (m_item == null)
                    return typeof(object);      // no properties to return nor type

                // return property type.
                return
                    ((Dictionary<string, object>)
                    typeof(T).InvokeMember(
                    "Properties",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    Type.DefaultBinder,
                    m_item,
                    null))[Name].GetType();
            }
        }
    }
}
