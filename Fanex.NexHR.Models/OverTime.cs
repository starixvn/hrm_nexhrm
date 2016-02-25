namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public class Overtime
    {
        public int Id { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime OnDate { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public float Hour { get; set; }

        public int Status { get; set; }

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee ModifiedBy { get; set; }

        public int TotalMember { get; set; }

        public IList<OvertimeRecord> OvertimeRecords { get; set; }
    }
}
