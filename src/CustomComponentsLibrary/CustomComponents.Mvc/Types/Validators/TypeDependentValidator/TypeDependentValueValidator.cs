using CustomComponents.Mvc.Types.Validators.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomComponents.Mvc.Types.Validators.TypeDependentValidator
{
    public class TypeDependentValueValidator : TypeDependentValidatorBase, IClientValidatable
    {
        /// <summary>
        ///     Gets the instance type to call the static method
        /// </summary>
        public Type InstanceType { get; private set; }


        public TypeDependentValueValidator(TypeCompareOptions option, Type instanceType, string internalStaticProperty)
            : base(option, internalStaticProperty)
        {
            if ( instanceType == null )
                throw new ArgumentNullException("instanceType");

            InstanceType = instanceType;
        }



        /// <summary>
        ///     Get another property value (public and instance) from the same instance of the model
        /// </summary>
        protected override object GetDependentPropertyValue(ValidationContext validationContext)
        {
            return GetValueOfProperty();
        }

        /// <summary>
        ///     Returns client validation rules for that class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <returns>
        ///     The client validation rules for this validator.
        /// </returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = NewRule(typeof(TypeDependentValueValidator).Name, metadata);
            rule.ValidationParameters["value"] = GetValueOfProperty();
            yield return rule;
        }



        #region Helpers

        private object GetValueOfProperty()
        {
            var pi = InstanceType.GetProperty(OtherPropertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return pi.GetValue(null, null);
        }

        #endregion




    }
}
