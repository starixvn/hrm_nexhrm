
namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HROvertimeViewModel
    {
        public HROvertimeViewModel()
        {
            this.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.ToDate = DateTime.Now;
        }

        [Display(Name = "CreateAccount", ResourceType = typeof(Labels))]
        public int? CreatedBy { get; set; }

        public string CreatedName { get; set; }

        public int DateOption { get; set; }

        public int Status { get; set; } 

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
