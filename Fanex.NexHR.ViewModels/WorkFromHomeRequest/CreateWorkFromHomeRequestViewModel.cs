namespace Fanex.NexHR.ViewModels.WorkFromHomeRequest
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateWorkFromHomeRequestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public string Remark { get; set; }
    }
}
