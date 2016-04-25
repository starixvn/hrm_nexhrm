namespace Fanex.NexHR.Models.CompanyEvent
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CompanyEvent
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string Location { get; set; }

        public string CreatedBy { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
