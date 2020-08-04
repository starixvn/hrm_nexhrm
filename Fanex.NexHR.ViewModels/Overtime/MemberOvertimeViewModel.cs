namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class MemberOvertimeViewModel
    {
        public MemberOvertimeViewModel()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Now;
            Status = 0;
            FromDateOT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDateOT = DateTime.Now;
        }

        public int? OvertimeId { get; set; }

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

        // Use for OT Request tab
        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(Labels))]
        public DateTime FromDateOT { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(Labels))]
        public DateTime ToDateOT { get; set; }
    }
}
