namespace Fanex.NexHR.ViewModels.CheckInOutRequest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Core;
    using Core.Enums;
    using Models;
    using Resources;

    public class CheckInOutRequestViewModel : BaseRequestModel, IValidatableObject
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ForDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23)]
        public int Hour { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59)]
        public int Minute { get; set; }

        [Required]
        public short CheckInOutType { get; set; }

        public string Shift { get; set; }

        public int ForRequest => (int)GeneralRequestType.CheckInOut;

        public IList<DateTime> UnavailDates { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }

        public decimal TotalRequest { get; set; }

        public bool IsEdit { get; set; }

        public string EmailTemplate => Utilities.ReadTextFile(EmailTemplatePath);

        public string EmailTemplatePath { get; private set; }

        public string SubjectEmail => string.Format(
            "{0} {1} on {2}",
            IsEdit ? Messages.UpdatedCheckInOutEmailSubject : Messages.CheckInOutEmailSubject,
            Employee,
            ForDate.ToDateMonthYearFormatedDate());

        public bool IsStandbyAtWeeken => Shift.Contains("A2")
                                         && (ForDate.DayOfWeek == DayOfWeek.Saturday || ForDate.DayOfWeek == DayOfWeek.Sunday);

        public void GetTemplatePath()
        {
            EmailTemplatePath = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForCheckInOut);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Shift))
            {
                var validation = new ValidationResult(
                    "Invalid shift",
                    new[] { "Shift" });
                result.Add(validation);
            }

            if (ForDate.Date > DateTime.Today)
            {
                var validation = new ValidationResult(
                    Messages.NoFutureCheckInOutRequest,
                    new[] { "ForDate" });
                result.Add(validation);
            }

            if (string.IsNullOrEmpty(Shift))
            {
                var validation = new ValidationResult(
                    "Invalid shift",
                    new[] { "Shift" });
                result.Add(validation);
            }

            return result;
        }

        public void SetShiftIntoModel(Shift shift)
        {
            if (shift != null)
            {
                Hour = CheckInOutType == (int)RequestType.CheckOut
                    ? int.Parse(shift.BeginCheckOutTime.Split(':')[0])
                    : int.Parse(shift.BeginCheckInTime.Split(':')[0]);
                Minute = CheckInOutType == (int)RequestType.CheckOut
                            ? int.Parse(shift.BeginCheckOutTime.Split(':')[1])
                            : int.Parse(shift.BeginCheckInTime.Split(':')[1]);
                Shift = shift.DisplayName;
                InTime = shift.BeginCheckInTime;
                OutTime = shift.BeginCheckOutTime;
            }
        }
    }
}