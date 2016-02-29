namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class BaseRequestModel
    {
        public long Id { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public byte Status { get; set; }

        public int By { get; set; }

        public string Remark { get; set; }

        public string DisplayName { get; set; }
    }
}
