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

        [Required]
        public string Location { get; set; }

        public string CreatedBy { get; set; }

        public string Description { get; set; }

        public DateTime FromDateTime
        {
            get
            {
                return string.IsNullOrWhiteSpace(FromTime) 
                    ? FromDate 
                    : new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, int.Parse(FromTime.Substring(0, 2)), int.Parse(FromTime.Substring(3, 2)), 0);
            }
        }

        public DateTime ToDateTime
        {
            get
            {
                return string.IsNullOrWhiteSpace(ToTime)
                    ? ToDate
                    : new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, int.Parse(ToTime.Substring(0, 2)), int.Parse(ToTime.Substring(3, 2)), 0);
            }
        }
    }
}
