namespace Fanex.NexHR.ViewModels.CustomValidationAttribute
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MultipleOfAttribute : ValidationAttribute, IClientValidatable
    {
        public int Value { get; set; }

        public override bool IsValid(object value)
        {
            int convertedValue = (int)value;

            return convertedValue % Value == 0;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "multipleof",
                ErrorMessage = string.Format(ErrorMessageString, metadata.GetDisplayName(), Value)
            };

            modelClientValidationRule.ValidationParameters.Add("value", Value);
            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}