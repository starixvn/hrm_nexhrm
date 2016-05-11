namespace Fanex.NexHR.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExtensionNoModel
    {
        public int EmpExtId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string Team { get; set; }

        public string TeamName { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        [Required]
        [Range(1000, 9999)]
        public string ExtensionNo { get; set; }

        public int OrderNoGroup { get; set; }

        public int OrderNoMember { get; set; }

        public bool IsInternal { get; set; }

        public int EmpId { get; set; }

        public string Employee { get; set; }

        public int TotalMember { get; set; }
    }
}
