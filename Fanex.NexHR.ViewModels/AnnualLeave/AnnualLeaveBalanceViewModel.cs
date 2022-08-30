namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System.ComponentModel.DataAnnotations;

    public class AnnualLeaveBalanceViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeNickName { get; set; }

        public int Year { get; set; }
        public int YearEdit { get; set; }

        public int Month { get; set; }


        [Display(Name = "Include employee's request")]
        public bool ExcludedRequestTrans { get; set; }
    }
}