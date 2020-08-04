
namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HROvertimeViewModel
    {
        public HROvertimeViewModel()
        {
            this.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.ToDate = DateTime.Now;
            this.Verified = (int)OvertimeVerifyType.All;
        }

        [Display(Name = "CreateAccount", ResourceType = typeof(Labels))]
        public int? CreatedBy { get; set; }

        public string CreatedName { get; set; }

        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int DateOption { get; set; }

        public int Verified { get; set; } 

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
