using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Fanex.NexHR.Core.Attributes
{
    public class TrainingDateValidatorAttribute : ValidationAttribute
    {
        private const string Certificates = "Certificates";
        private const string pattern = @"(^(((0[1-9]|1[0-9]|2[0-8])[\/](0[1-9]|1[012]))|((29|30|31)[\/](0[13578]|1[02]))|((29|30)[\/](0[4,6,9]|11)))[\/](19|[2-9][0-9])\d\d$)|(^29[\/]02[\/](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                string strValue = value.ToString();
                var toDate = DateTime.Parse(strValue);
                Match match = Regex.Match(toDate.ToString("dd/MM/yyyy"), pattern);
                if (match.Success)
                {
                    var propertyFromDate = validationContext.ObjectType.GetProperty("FromDate");
                    if (propertyFromDate == null)
                        return new ValidationResult(string.Format("Property '{0}' is Null", "FromDate"));
                    var objFromDate = propertyFromDate.GetValue(validationContext.ObjectInstance, null);
                    string strFromdate = objFromDate == null ? "" : objFromDate.ToString();
                    var fromDate = DateTime.Parse(strFromdate);
                    Match matchJoiningDate = Regex.Match(fromDate.ToString("dd/MM/yyyy"), pattern);
                    if (matchJoiningDate.Success && fromDate > toDate)
                    {
                        var propertyTypeName = validationContext.ObjectType.GetProperty("TypeName");
                        var objTypeName = propertyTypeName.GetValue(validationContext.ObjectInstance, null);
                        string strTypeName = objTypeName == null ? "" : objTypeName.ToString();
                        if (strTypeName == Certificates)
                        {
                            return new ValidationResult("Test Date must be less than or equal Expiration Date.");

                        }
                        else
                        {
                            return new ValidationResult("Start Date must be less than or equal End Date.");
                        }
                    }
                }
                else
                    return new ValidationResult("Invalid date format");
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult("Invalid date format");
            }
        }
    }
}
