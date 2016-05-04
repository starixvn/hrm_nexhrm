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
                if (FromTime != string.Empty && FromTime != null)
                {
                    return new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, int.Parse(FromTime.Substring(0, 2)), int.Parse(FromTime.Substring(3, 2)), 0);
                }
                else
                {
                    return FromDate;
                }
            }
        }

        public DateTime ToDateTime
        {
            get
            {
                if (ToTime != string.Empty && ToTime != null)
                {
                    return new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, int.Parse(ToTime.Substring(0, 2)), int.Parse(ToTime.Substring(3, 2)), 0);
                }
                else
                {
                    return ToDate;
                }
            }
        }
    }
}
