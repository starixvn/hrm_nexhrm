using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fanex.NexHR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrainingDateValidatorAttribute : ValidationAttribute
    {
        private const string Certificates = "Certificates";
        private const string ErrorEndDateLessStartDate = "Test Date must be less than or equal Expiration Date.";
        private const string ErrorExpirationDateLessTestDate = "Start Date must be less than or equal End Date.";
        private const string pattern = @"(^(((0[1-9]|1[0-9]|2[0-8])[\/](0[1-9]|1[012]))|((29|30|31)[\/](0[13578]|1[02]))|((29|30)[\/](0[4,6,9]|11)))[\/](19|[2-9][0-9])\d\d$)|";
        private const string patternSecond = @"(^29[\/]02[\/](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)";
        private const string InvalidDateFormat = "Invalid date format";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value is null)
                {
                    if (IsCertificate(validationContext))
                    {
                        var propertyFromDate = validationContext.ObjectType.GetProperty("FromDate");
                        return propertyFromDate == null ? new ValidationResult(InvalidDateFormat) : ValidationResult.Success;
                    }
                    return new ValidationResult(InvalidDateFormat);
                }

                string strValue = value.ToString();
                var toDate = DateTime.Parse(strValue);
                Match match = Regex.Match(toDate.ToString("dd/MM/yyyy"), pattern + patternSecond);
                if (match.Success)
                {
                    var propertyFromDate = validationContext.ObjectType.GetProperty("FromDate");
                    if (propertyFromDate == null)
                    {
                        return new ValidationResult(string.Format("Property '{0}' is Null", "FromDate"));
                    }
                    Match matchJoiningDate = Regex.Match(GetFromDate(propertyFromDate, validationContext).ToString("dd/MM/yyyy"), pattern + patternSecond);
                    if (matchJoiningDate.Success && GetFromDate(propertyFromDate, validationContext) > toDate)
                    {
                        return new ValidationResult(GetMessage(validationContext));
                    }
                }
                else
                {
                    return new ValidationResult(InvalidDateFormat);
                }
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult(InvalidDateFormat);
            }
        }

        private static string GetMessage(ValidationContext validationContext)
        {
            var propertyTypeName = validationContext.ObjectType.GetProperty("TypeName");
            var objTypeName = propertyTypeName.GetValue(validationContext.ObjectInstance, null);
            string strTypeName = objTypeName == null ? "" : objTypeName.ToString();
            return strTypeName.Equals(Certificates) ? ErrorEndDateLessStartDate : ErrorExpirationDateLessTestDate;
        }

        private static bool IsCertificate(ValidationContext validationContext)
        {
            var propertyTypeName = validationContext.ObjectType.GetProperty("TypeName");
            var objTypeName = propertyTypeName.GetValue(validationContext.ObjectInstance, null);
            string strTypeName = objTypeName == null ? "" : objTypeName.ToString();
            return strTypeName.Equals(Certificates);
        }

        private static DateTime GetFromDate(PropertyInfo propertyFromDate, ValidationContext validationContext)
        {
            var objFromDate = propertyFromDate.GetValue(validationContext.ObjectInstance, null);
            string strFromdate = objFromDate == null ? "" : objFromDate.ToString();
            var fromDate = DateTime.Parse(strFromdate);
            return fromDate;
        }
    }
}
