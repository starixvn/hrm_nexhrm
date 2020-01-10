namespace Fanex.NexHR.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExtensionNoModel
    {
        const int minRange = 1000;
        const int maxRange = 9999;

        public int EmpExtId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string Team { get; set; }

        public string TeamName { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        [Required]
        [Range(minRange, maxRange)]
        public string ExtensionNo { get; set; }

        public int OrderNoGroup { get; set; }

        public int OrderNoMember { get; set; }

        public bool IsInternal { get; set; }

        public int EmpId { get; set; }

        public string Employee { get; set; }

        public int TotalMember { get; set; }
    }
}
