namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;

    public class ApplicantViewModel
    {

        public ApplicantViewModel()
        {
            this.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.ToDate = DateTime.Now;
        }

        public int PositionId { get; set; }

        public int Status { get; set; }

        public string FullName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}