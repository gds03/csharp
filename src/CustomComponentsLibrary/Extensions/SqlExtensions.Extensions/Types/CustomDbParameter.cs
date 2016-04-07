using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExtensions.Extensions.Types
{
    /// <summary>
    ///     Create instances of this type, when you don't care about setting the type of
    ///     the DbParameter. 
    ///     Call ToDbParameters in a Array of this instances, and the method will take care about setting the
    ///     correct type.
    /// </summary>
    public sealed class CustomDbParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public CustomDbParameter(string Name, object Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
