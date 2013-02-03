using System;
using System.Collections.Generic;

namespace EnhancedLibrary.ExternalTypes.Business.DynamicData
{
    public interface IDynamicObject
    {
        /// <summary>
        ///     Get the dictionary that represents the name of the properties and their values in runtime 
        /// </summary>
        Dictionary<string, object> Properties { get; }      // Do not change the name and return type of this property. It is statically called by reflection to get the type of each property


                                                                                                                                                                                                                

        /// <summary>
        ///     Removes all properties
        /// </summary>
        void Clear();





        /// <summary>
        ///     Get or set the value of some property
        /// </summary>
        object this[string propertyName] { get; set; }




        /// <summary>
        ///     Try to get the value of the property.
        /// </summary>
        /// <param name="result">out parameter that contains null or the value of the property if property was found or not</param>
        /// <returns>true if property was found. Otherwise return false</returns>
        bool TryGetValue(string propertyName, out object result);




        /// <summary>
        ///     Set the property value and a method that is called before the value changed
        /// </summary>
        void SetPropertyValueAndEvent(string propertyName, object propertyValue, Action<string, object> beforePropertyChangedEventHandler);



        /// <summary>
        ///     Set the property value and a method that is called after the value changed
        /// </summary>
        void SetPropertyValueAndEvent(string propertyName, object propertyValue, Action<string, object> beforePropertyChangedEventHandler, Action<string, object> afterPropertyChangedEventHandler);
    }
}
