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
    public class TypeDependentPropertyValidator : TypeDependentValidatorBase, IClientValidatable
    {
        public TypeDependentPropertyValidator(TypeCompareOptions option, string otherPropertyName)
            : base(option, otherPropertyName)
        {

        }

        /// <summary>
        ///     Get another property value (public and instance) from the same instance of the model
        /// </summary>
        protected override object GetDependentPropertyValue(ValidationContext validationContext)
        {
            var pi = validationContext.ObjectInstance.GetType().GetProperty(OtherPropertyName, BindingFlags.Public | BindingFlags.Instance);
            return pi.GetValue(validationContext.ObjectInstance, null);
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
            var rule = NewRule(typeof(TypeDependentPropertyValidator).Name, metadata);
            yield return rule;
        }
    }
}
