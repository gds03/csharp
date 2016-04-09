using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.OMapper.Internal
{
    /// <summary>
    ///     A class that holds the object returned by OMapper and the hash for all properties.
    ///     It contains also a method that returns the updated/changed properties names of that object.
    /// </summary>
    class SelectObjectInfo
    {
        public object Object { get; private set; }

        public int[] PropertiesHashInfo { get; private set; }


        public SelectObjectInfo(object initialStateObj)
        {
            if (initialStateObj == null)
                throw new ArgumentNullException("initialStateObj");

            Object = initialStateObj;
            Type t = initialStateObj.GetType();
            PropertyInfo[] properties = t.GetProperties(OMapper.s_PropertiesFlags);
            PropertiesHashInfo = new int[properties.Length];

            int i = 0;
            foreach (var pi in properties)
            {
                object v = pi.GetValue(initialStateObj, null);
                int hash = (v == null) ? 0 : v.GetHashCode();
                PropertiesHashInfo[i] = hash;
                i++;
            }
        }

        public string[] GetPropertiesChanged()
        {
            if (Object == null)
                return null;

            Type t = Object.GetType();
            PropertyInfo[] properties = t.GetProperties(OMapper.s_PropertiesFlags);
            List<string> changedProperties = new List<string>();

            int i = 0;
            foreach (var pi in properties)
            {
                object v = pi.GetValue(Object, null);
                int hash = (v == null) ? 0 : v.GetHashCode();
                if (PropertiesHashInfo[i] != hash)
                    changedProperties.Add(pi.Name);

                i++;
            }

            return (changedProperties.Count == 0) ? null : changedProperties.ToArray();
        }
    }
}
