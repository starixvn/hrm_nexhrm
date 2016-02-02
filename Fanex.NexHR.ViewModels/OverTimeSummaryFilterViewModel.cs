namespace Fanex.NexHR.ViewModels
{
    using Core.Enums;
    using Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OvertimeSummaryFilterViewModel
    {
        public OvertimeSummaryFilterViewModel()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            Status = (int)RequestStatuse.Pending;
        }

        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(Labels))]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(Labels))]
        public DateTime ToDate { get; set; }

        [Display(Name = "CreateAccount", ResourceType = typeof(Labels))]
        public int CreatedBy { get; set; }

        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Labels))]
        public int Status { get; set; }       
    }
}
