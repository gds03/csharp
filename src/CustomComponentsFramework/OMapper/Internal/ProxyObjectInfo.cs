using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OMapper.Internal
{
    /// <summary>
    ///     A class that holds the object returned by OMapper and the hash for all properties.
    ///     It contains also a method that returns the updated/changed properties names of that object.
    /// </summary>
    internal class ProxyObjectInfo
    {
        internal object ProxyObject { get; private set; }

        public Dictionary<string, bool> PropertiesChanged { get; private set; }


        public ProxyObjectInfo(object initialStateObj)
        {
            if (initialStateObj == null)
                throw new ArgumentNullException("initialStateObj");

            ProxyObject = initialStateObj;
            PropertiesChanged = new Dictionary<string, bool>();
        }
    }
}
