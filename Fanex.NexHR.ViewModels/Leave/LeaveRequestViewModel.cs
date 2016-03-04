namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class LeaveRequestViewModel : BaseRequestModel, IValidatableObject
    {
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

        public int LeaveTypeId { get; set; }

        public bool IsPlanned { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool HalfDayOff { get; set; }

        public bool IsInsteadLateEarlyOut { get; set; }

        public string LeaveTypeName { get; set; }

        public int ForRequest
        {
            get
            {
                return (int)GeneralRequestType.Leave;
            }
        }

        public IList<LeaveType> LeaveTypes { get; set; }

        public IList<DateTime> UnavailDates { get; set; }

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

            return result;
        }
    }
}