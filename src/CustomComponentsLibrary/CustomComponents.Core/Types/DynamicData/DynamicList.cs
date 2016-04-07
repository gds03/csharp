using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using CustomComponents.Core.Interfaces;

namespace CustomComponents.Core.Types.DynamicData
{
    public class DynamicList<T> : List<T>, ITypedList 
        where T : IDynamicObject
    {
        int bindingIndex = 0;


        public string GetListName(PropertyDescriptor[] listAccessors) { return typeof(DynamicList<T>).Name; }
        
        // Called for each item in the list. As the method don't receive the index, we use bindingIndex
        // to know which item we are getting.
        // This method is called Count + 1, and in the last time, we set bindingIndex to 0.
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length != 0)
                throw new NotSupportedException();

            if (Count == 0)
                return new PropertyDescriptorCollection(new[] { new DynamicObjectPropertyDescriptor<T>(default(T), "-") });

            T item = this[bindingIndex];
            PropertyDescriptorCollection result = new PropertyDescriptorCollection(item.Properties.Keys.Select(prop => new DynamicObjectPropertyDescriptor<T>(item, prop)).ToArray());
            
            if (++bindingIndex == Count)
                bindingIndex = 0;

            return result;
        }

        
    }
}
