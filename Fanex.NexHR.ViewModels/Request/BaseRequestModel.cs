namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class BaseRequestModel
    {
        public long Id { get; set; }

        [Required]
        public string Employee { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "InvalidEmployeeID", ErrorMessageResourceType = typeof(Messages))]
        public int EmployeeId { get; set; }

        public byte Status { get; set; }

        public int By { get; set; }

        public string Remark { get; set; }
    }
}
