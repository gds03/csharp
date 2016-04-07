using CustomComponents.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace CustomComponents.Core.Types.DynamicData
{
    public class DynamicObject : IDynamicObject
    {
        sealed class PropertyNode
        {
            internal object                 m_value;
            internal Action<string, object> m_beforeValueChanged;
            internal Action<string, object> m_afterValueChanged;
        }


        // Dictionary that maps strings to PropertyNodes
        readonly Dictionary<string, PropertyNode> m_properties = new Dictionary<string, PropertyNode>();









        #region Internal Methods


        /// <summary>
        ///     Return null if property is not found. Otherwise return the node that contains the property
        /// </summary>
        PropertyNode GetNode(string propertyName)
        {
            PropertyNode result;

            if ( !m_properties.TryGetValue(propertyName, out result) )
                return null;

            return result;
        }



        void _SetPropertyValueAndEvent(string propertyName, object propertyValue, Action<string, object> beforePropertyChangedEventHandler, Action<string, object> afterPropertyChangedEventHandler)
        {
            // If property don't exists, creates one;
            // If exists, update the node

            PropertyNode node = GetNode(propertyName);

            if ( node == null )
            {
                node = new PropertyNode() { m_value = propertyValue, m_beforeValueChanged = beforePropertyChangedEventHandler, m_afterValueChanged = afterPropertyChangedEventHandler };
                m_properties.Add(propertyName, node);
            }

            else
            {
                node.m_value = propertyValue;
                node.m_beforeValueChanged = beforePropertyChangedEventHandler;
                node.m_afterValueChanged = afterPropertyChangedEventHandler;
            }
        }




        #endregion









        /// <summary>
        ///     Get the dictionary that represents the name of the properties and their values in runtime 
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                Dictionary<string, object> result = new Dictionary<string, object>(m_properties.Count);

                foreach(var keyEntry in result.Keys)
                {
                    // Fill the result dictionary
                    result.Add(keyEntry, m_properties[keyEntry]);
                }

                return result;
            }
        }
        

        /// <summary>
        ///     Removes all properties
        /// </summary>
        public void Clear() { m_properties.Clear(); }
        
        

        public object this[string propertyName]
        {
            get
            {
                PropertyNode node = GetNode(propertyName);

                if ( node == null )
                    throw new KeyNotFoundException(string.Format("Property with name: {0} is not defined in the object", propertyName));

                return node.m_value;
            }

            set
            {
                // If node is not present we need to create one
                // If exists, update the value
                PropertyNode node = GetNode(propertyName);

                if ( node == null )
                {
                    node = new PropertyNode() { m_value = value };
                    m_properties.Add(propertyName, node);                       // Add a new node to the dictionary
                }

                else
                {
                    //
                    // Whetever this node defines the event for updating the current value, invoke that event handler

                    if ( node.m_beforeValueChanged != null )
                        node.m_beforeValueChanged(propertyName, node.m_value);  // invoke with old value

                    node.m_value = value;                                       // Update existing node value


                    if ( node.m_afterValueChanged != null )
                        node.m_afterValueChanged(propertyName, value);          // invoke with new value
                }
            }
        }

        

        public void SetPropertyValueAndEvent(string propertyName, object propertyValue, Action<string, object> beforePropertyChangedEventHandler)
        {
            _SetPropertyValueAndEvent(propertyName, propertyValue, beforePropertyChangedEventHandler, null);
        }

        public void SetPropertyValueAndEvent(string propertyName, object propertyValue, Action<string, object> beforePropertyChangedEventHandler, Action<string, object> afterPropertyChangedEventHandler)
        {
            _SetPropertyValueAndEvent(propertyName, propertyValue, beforePropertyChangedEventHandler, afterPropertyChangedEventHandler);
        }




        /// <summary>
        ///     Try to get the value of the property.
        /// </summary>
        /// <param name="result">out parameter that contains null or the value of the property if property was found or not</param>
        /// <returns>true if property was found. Otherwise return false</returns>
        public bool TryGetValue(string propertyName, out object result)
        {
            PropertyNode node = GetNode(propertyName);
            result = null;

            if ( node == null )
                return false;

            result = node.m_value;
            return true;
        }





        








        
    }
}
