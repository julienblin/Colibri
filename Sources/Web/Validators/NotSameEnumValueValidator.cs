using System;
using Castle.Components.Validator;

namespace CDS.Framework.Tools.Colibri.Web.Validators
{
    public class NotSameEnumValueValidator : NotSameValueValidator
    {
        public NotSameEnumValueValidator(object value)
            : base(value)
        {
        }

        public override bool IsValid(object instance, object fieldValue)
        {
            return base.IsValid(instance, fieldValue);
        }
    }

    public class ValidateNotSameEnumValueAttribute : ValidateNotSameValueAttribute
    {
        private readonly string value;

        public ValidateNotSameEnumValueAttribute(Type enumType, object mustNotBeThisValue)
            : base(enumType, mustNotBeThisValue)
        {
            value = ((int)mustNotBeThisValue).ToString();
        }

        public ValidateNotSameEnumValueAttribute(Type enumType, object mustNotBeThisValue, string errorMessage)
            : base(enumType, mustNotBeThisValue, errorMessage)
        {
            value = ((int)mustNotBeThisValue).ToString();
        }

        public override IValidator Build()
        {
            IValidator validator = new NotSameEnumValueValidator(value);
            ConfigureValidatorMessage(validator);
            return validator;
        }
    }
}
