namespace Fanex.NexHR.ViewModels.WorkFromHomeRequest
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateWorkFromHomeRequestByHRViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Employee is required")]
        public int EmpId { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public string Remark { get; set; }
    }
}
