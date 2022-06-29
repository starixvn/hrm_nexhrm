namespace Fanex.NexHR.ViewModels.CustomValidationAttribute
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MultipleOfAttribute : ValidationAttribute
    {
        public int Value { get; set; }

        public override bool IsValid(object value)
        {
            int convertedValue = (int)value;

            return convertedValue % Value == 0;
        }
    }
}