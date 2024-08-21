namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System.ComponentModel.DataAnnotations;

    public class EmployeeAnnualLeaveViewModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public int Year { get; set; }

        [Display(Name = "Include employee's request")]
        public bool IncludeRequestTrans { get; set; }
    }
}