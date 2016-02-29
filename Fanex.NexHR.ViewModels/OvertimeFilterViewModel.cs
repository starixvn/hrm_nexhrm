namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Enums;
    using Resources;

    public class MemberOvertimeViewModel
    {
        public MemberOvertimeViewModel()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Now;
            Status = 0;
        }

        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(Labels))]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(Labels))]
        public DateTime ToDate { get; set; }

        [Display(Name = "CreateAccount", ResourceType = typeof(Labels))]
        public int? CreatedBy { get; set; }

        public string CreatedName { get; set; }

        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Labels))]
        public int Status { get; set; }
    }
}
