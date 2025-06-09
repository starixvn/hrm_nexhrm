
namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HROvertimeViewModel
    {
        public HROvertimeViewModel()
        {
            this.FromDate = new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, DateTimeOffset.Now.DateTime.ToClientTimeZone().Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            this.ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
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
