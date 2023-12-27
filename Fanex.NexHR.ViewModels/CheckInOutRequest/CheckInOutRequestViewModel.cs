namespace Fanex.NexHR.ViewModels.CheckInOutRequest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core;
    using Core.Enums;
    using Models;
    using Resources;

    public class CheckInOutRequestViewModel : BaseRequestModel, IValidatableObject
    {
        private const byte MaxHour = 23;

        private const byte MaxMinute = 59;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }

        public bool IsCrossDate { get; set; }

        [Required]
        [Range(0, MaxHour)]
        public int Hour { get; set; }

        [Required]
        [Range(0, MaxMinute)]
        public int Minute { get; set; }

        [Required]
        public short CheckInOutType { get; set; }

        public string RequestTypeName { get; set; }

        public string Shift { get; set; }

        public int ForRequest { get; set; } = (int)GeneralRequestType.CheckInOut;

        public IEnumerable<DateTime> UnavailDates { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }

        public decimal TotalRequest { get; set; }

        public bool IsEdit { get; set; }

        public string EmailTemplate => Utilities.ReadTextFile(EmailTemplatePath);

        public string EmailTemplatePath { get; set; }

        public string SubjectEmail => string.Format(
            "{0} {1} on {2}",
            IsEdit ? Messages.UpdatedCheckInOutEmailSubject : Messages.CheckInOutEmailSubject,
            Employee,
            WorkDate.ToDateMonthYearFormatedDate());

        public bool IsStandbyAtWeekend => (WorkDate.DayOfWeek == DayOfWeek.Saturday || WorkDate.DayOfWeek == DayOfWeek.Sunday);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (WorkDate.Date > DateTimeOffset.Now.DateTime.Date)
            {
                var validation = new ValidationResult(
                    Messages.NoFutureCheckInOutRequest,
                    new[] { "ForDate" });
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