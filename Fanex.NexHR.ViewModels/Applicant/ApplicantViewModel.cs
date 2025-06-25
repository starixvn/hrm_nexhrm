namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Applicant;
    using Fanex.NexHR.Core;

    public class ApplicantViewModel
    {
        private const int month = -3;
        public ApplicantViewModel()
        {
            FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().AddMonths(month).Date;
            ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
            PageNumber = 1;
        }

        public string FullName { get; set; }

        public int PositionId { get; set; }

        public int Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public IEnumerable<Position> Positions { get; set; }

        public IEnumerable<Source> StatusList { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRow { get; set; }
    }
}