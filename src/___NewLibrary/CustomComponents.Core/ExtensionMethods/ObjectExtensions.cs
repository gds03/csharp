using CustomComponents.Core.Types;
using CustomComponents.Core.Types.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Convert the current object to a Dictionary
        /// </summary>        
        public static Dictionary<string, object> ToDictionary(this object data)
        {
            if (data == null)
                return null;

            Dictionary<string, object> d = data as Dictionary<string, object>;
            if (d != null)
                return d;        // already a dictionary

            BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo property in data.GetType().GetProperties(publicAttributes))
            {
                if (property.CanRead)
                {
                    dictionary.Add(property.Name, property.GetValue(data, null));
                }
            }
            return dictionary;
        }
        /// <summary>
        ///     Throws an ArgumentNullException if obj is null.
        ///     Should be only used to verify method parameters
        /// </summary>
        public static void CheckArgumentNullAndThrow(this object obj, string argName)
        {
            if (obj == null)
                throw new ArgumentNullException(argName);
        }

        /// <summary>
        ///     Throws an ArgumentNullException if obj is null.
        ///     Should be only used to verify method parameters
        /// </summary>
        public static void CheckArgumentNullAndThrow<ArgumentT>(this object obj, Expression<Func<ArgumentT>> argumentSelector)
        {
            if (obj == null)
            {
                string argName = NamesResolver.Variable(argumentSelector);
                throw new ArgumentNullException(argName);
            }
        }


        /// <summary>
        ///     Throws an InvalidOperationException of obj is null.
        /// </summary>
        public static void CheckVarAndThrow(this object obj, string msg = null)
        {
            if (obj == null)
                throw new InvalidOperationException(msg);
        }


        /// <summary>
        ///     Throws an InvalidOperationException of obj is null.
        /// </summary>
        public static void CheckVarAndThrow<VariableT>(this object obj, Expression<Func<VariableT>> variableSelector, string msg = null)
        {
            if (obj == null)
            {
                string m = (msg != null) ? msg : (NamesResolver.Variable(variableSelector) + " should't be NULL for expected state of the application");
                throw new InvalidOperationException(msg);
            }
        }



        /// <summary>
        ///     Validates if current object extends from type
        /// </summary>
        public static bool IsOfType(this object obj, Type type)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(Object))
                throw new InvalidOperationException("all types are derived from object");

            Type t = obj.GetType();
            while (t != typeof(object) && t != type)
            {
                Type possibleInterface = t.GetInterfaces().SingleOrDefault(x => x.FullName == type.FullName);

                if (possibleInterface != null)
                {
                    t = possibleInterface;
                    break;
                }

                t = t.BaseType;
            }

            return (t == type);
        }



        /// <summary>
        /// Serializes the object to XML.
        /// </summary>
        /// <param name="objToXml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string SerializeXml(this Object objToXml, Type type = null)
        {
            using (var memStream = new MemoryStream())
            {
                var dcs = new DataContractSerializer(type ?? objToXml.GetType());
                using (var stWriter = new StreamWriter(memStream, Encoding.ASCII))
                using (XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(memStream, Encoding.UTF8))
                    dcs.WriteObject(xdw, objToXml);

                return Encoding.ASCII.GetString(memStream.ToArray());
            }
        }
    }
}
