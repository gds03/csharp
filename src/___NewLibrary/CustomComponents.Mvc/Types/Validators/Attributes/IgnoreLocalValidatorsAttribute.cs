using CustomComponents.Mvc.Types.Validators.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Mvc.Types.Validators.Attributes
{
    /// <summary>
    ///     Indicate to the local validators that the current property isn't to be validated.
    /// </summary>
    public interface IIgnoreValidator
    {
        bool ConditionSatisfied(object currentInstance);
    }

    /// <summary>
    ///     Indicate to the local validators that this property is only to validate when condition is satisfied.
    /// </summary>
    public class IgnoreLocalValidatorsAttribute : Attribute, IIgnoreValidator
    {
        public string PropertyName { get; private set; }
        public object PropertyValue { get; private set; }
        public TypeCompareOptions Option { get; private set; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="propertyName">The value of the property to compare</param>
        /// <param name="value">expected value</param>
        /// <param name="option">signal</param>
        public IgnoreLocalValidatorsAttribute(string propertyName, object value, TypeCompareOptions option)
        {
            if ( propertyName == null )
                throw new ArgumentNullException("propertyName");

            PropertyName = propertyName;
            PropertyValue = value;
            Option = option;
        }


        public bool ConditionSatisfied(object currentInstance)
        {
            var pi = currentInstance.GetType().GetProperty(PropertyName);
            object val = pi.GetValue(currentInstance, null);

            return CompareHelper.Compare(val, PropertyValue, Option);
        }
    }
}
