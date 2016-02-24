using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web.Mvc;
using CustomComponents.Mvc.Types.Validators.Helpers;
using CustomComponents.Mvc.Types.Validators.Attributes;
using CustomComponents.Core.Types;

namespace CustomComponents.Mvc.Types.Validators.TypeDependentValidator
{
    public abstract class TypeDependentValidatorBase : ValidationAttribute
    {
        const string invalidMsg = "Invalid validation.";

        /// <summary>
        ///     Gets the name of the  property to compare to
        /// </summary>
        public string OtherPropertyName { get; private set; }

        /// <summary>
        ///     Gets the signal to compare the two fields
        /// </summary>
        public TypeCompareOptions Option { get; private set; }


        public TypeDependentValidatorBase(TypeCompareOptions option, string otherPropertyName)
        {
            Option = option;

            if ( string.IsNullOrEmpty(otherPropertyName) )
                throw new ArgumentNullException("otherPropertyName");

            OtherPropertyName = otherPropertyName;
        }

        /// <summary>
        ///     Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        ///     An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 
            // check immediately if is to validate or not the field.

            IIgnoreValidator ignoreValidator = null;
            if ( IsIgnoreValidatorFound(validationContext, out ignoreValidator) && ignoreValidator.ConditionSatisfied(validationContext.ObjectInstance) )
                return ValidationResult.Success;

            object otherPiValue = GetDependentPropertyValue(validationContext);
            otherPiValue = CheckIfIsMyDateTime(otherPiValue);                       // this should't be here.

            if ( value == null || otherPiValue == null )
                return ValidationResult.Success;            // assumed that always valid either if they are null. To protect this, use [Required] attribute before            

            if ( otherPiValue.GetType() != value.GetType() )
                throw new InvalidOperationException("Types are not compatible. Different types detected for comparasion");

            bool isValid = CompareHelper.Compare(value, otherPiValue, Option);

            if ( !isValid )
                return new ValidationResult(CustomizedErrorMessage);

            // 
            // every thing is OK so return Success.

            return ValidationResult.Success;
        }

        private bool IsIgnoreValidatorFound(ValidationContext validationContext, out IIgnoreValidator ignoreValidator)
        {
            ignoreValidator = null;
            string propertyName = validationContext.MemberName;

            if ( propertyName == null )
            {
                PropertyInfo p = validationContext.ObjectType.GetProperties()
                                                             .FirstOrDefault(pi => pi.GetCustomAttributes<DisplayAttribute>().Any(da => da.GetName() == validationContext.DisplayName));

                if ( p != null )
                    propertyName = p.Name;
            }

            if ( string.IsNullOrEmpty(propertyName) )
                return false;

            PropertyInfo propInfo = validationContext.ObjectInstance.GetType().GetProperty(propertyName);
            ignoreValidator = propInfo.GetCustomAttribute<IgnoreLocalValidatorsAttribute>();

            return ignoreValidator != null;
        }


        /// <summary>
        ///     If value is of MyDateTime type, get the inner datetime field,
        ///     otherwise return the same object.
        /// </summary>
        private object CheckIfIsMyDateTime(object value)
        {
            if ( value != null && value.GetType() == typeof(UserDateTime) )
            {
                // transform in DateTime
                return ((UserDateTime) value).Date;
            }
            return value;
        }


        /// <summary>
        ///     Hook method that concrete classes must implement to return the value of the property to compare.
        /// </summary>
        protected abstract object GetDependentPropertyValue(ValidationContext validationContext);



        #region Helper Methods

        /// <summary>
        ///     Helper method to generate a string for simple types and for generic types
        /// </summary>
        protected static string GetModelType(Type type)
        {
            if ( type == null )
                throw new ArgumentNullException("type");

            if ( type.GenericTypeArguments.Length == 0 )
                return type.Name;

            var text = type.GenericTypeArguments.Aggregate(new StringBuilder("<"), (sb, t) => sb.Append(t.Name + ", "));
            text = text.Remove(text.Length - 2, 2);
            text.Append(">");

            // substitute example: Nullable`1 for Nullable<Datetime>
            string tp = type.Name;
            return (tp.Substring(0, tp.IndexOf("`")) + text.ToString()).Trim();
        }

        /// <summary>
        ///     Return a formatted string that indicate what is the value and description for comparation.
        /// </summary>
        protected static string SignalDescriptors
        {
            get
            {
                var t = typeof(TypeCompareOptions);
                var descriptors = Enum.GetNames(t).Aggregate(new StringBuilder(), (sb, s) => sb.Append(s + "=" + ((int) Enum.Parse(t, s)) + ", "));
                return descriptors.Remove(descriptors.Length - 2, 2).ToString();
            }
        }

        /// <summary>
        ///     Helper that returns a string default for error message if not passed, otherwise return user defined string.
        /// </summary>
        protected string CustomizedErrorMessage
        {
            get
            {
                if ( string.IsNullOrEmpty(ErrorMessageString) )
                    return invalidMsg;

                return ErrorMessageString;
            }
        }

        /// <summary>
        ///     Creates a new Rule defined with default parameters.
        /// </summary>
        protected ModelClientValidationRule NewRule(string validatorName, ModelMetadata metadata)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule
            {
                ErrorMessage = CustomizedErrorMessage,
                ValidationType = validatorName.ToLower().Trim()
            };

            rule.ValidationParameters["otherpropertyname"] = OtherPropertyName;
            rule.ValidationParameters["modeltype"] = GetModelType(metadata.ModelType);
            rule.ValidationParameters["signal"] = (int) Option;
            rule.ValidationParameters["signaldescriptor"] = SignalDescriptors;

            return rule;
        }


        #endregion
    }
}
