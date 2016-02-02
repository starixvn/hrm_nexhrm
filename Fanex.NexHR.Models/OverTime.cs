using Fanex.NexHR.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Models
{
    public class Overtime
    {
        public int Id { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public float Hour { get; set; }

        public int Status { get; set; }
        public Employee CreatedBy { get; set; }
        public Employee ModifiedBy { get; set; }        
        public Employee Employee { get; set; }

    }
}
