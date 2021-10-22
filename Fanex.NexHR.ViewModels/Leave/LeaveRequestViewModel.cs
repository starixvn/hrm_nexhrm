namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Core;
    using Core.Enums;
    using Models;
    using Resources;

    public class LeaveRequestViewModel : BaseRequestModel, IValidatableObject
    {
        private const int numberDay = 5;

        private const byte MaxHour = 23;

        private const byte MaxMinute = 59;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.##}")]
        public decimal TotalDays { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxHour)]
        public int FromHour { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxHour)]
        public int ToHour { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxMinute)]
        public int FromMinute { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaxMinute)]
        public int ToMinute { get; set; }

        public int LeaveTypeId { get; set; }

        public bool IsPlanned { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool HalfDayOff { get; set; }

        public bool IsInsteadLateEarlyOut { get; set; }

        public string LeaveTypeName { get; set; }

        public int ForRequest { get; set; } = (int)GeneralRequestType.Leave;

        public decimal TotalRequest { get; set; }

        public string CurrentUserEmail { get; set; }

        public IList<LeaveType> LeaveTypes { get; set; }

        public IList<DateTime> UnavailDates { get; set; }

        public bool IsUnpaidLeave => LeaveTypeId == Constants.UnpaidLeaveType;

        public bool IsAnnualLeaveOver5Day => (LeaveTypeId == Constants.AnnualLeaveType || LeaveTypeId == Constants.UnplannedAnnualLeaveType)
                                             && TotalDays >= numberDay;

        public bool IsEdit { get; set; }

        public string EmailTemplate => Utilities.ReadTextFile(EmailTemplatePath);

        public string EmailTemplatePath { get; private set; }

        public string SubjectEmail
        {
            get
            {
                string subjectTemplate = TotalDays > 1 ? "{0} {1} from {2} to {3}" : "{0} {1} on {2}";
                return string.Format(
                    subjectTemplate,
                    IsEdit ? Messages.UpdatedLeaveEmailSubject : Messages.LeaveEmailSubject,
                    Employee,
                    FromDate.ToDateMonthYearFormatedDate(),
                    ToDate.ToDateMonthYearFormatedDate());
            }
        }

        public void GetTemplatePath()
        {
            EmailTemplatePath = IsUnpaidLeave || IsAnnualLeaveOver5Day
                        ? HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForUnpaidLeave)
                        : HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForLeave);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            var diff = ToDate.Subtract(FromDate);
            if (diff.TotalDays < 0)
            {
                var validation = new ValidationResult(
                    Messages.InvalidToDate,
                    new[] { "ToDate" });
                result.Add(validation);
            }

            if (HalfDayOff && (TotalDays % 1 == 0))
            {
                var validation = new ValidationResult(
                    Messages.InvalidTotalDays,
                    new[] { "TotalDays" });
                result.Add(validation);
            }
            var isInValidTime = (FromHour > ToHour) || (FromHour == ToHour && FromMinute >= ToMinute);
            if (LeaveTypeId == Constants.BusinessTripLeaveType && HalfDayOff && isInValidTime)
            {
                var validation = new ValidationResult(
                Messages.InvalidToDate,
                new[] { "ToDate" });
                result.Add(validation);
            }

            return result;
        }
    }
}