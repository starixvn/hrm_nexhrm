namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ApplicantViewModel
    {
        public ApplicantViewModel()
        {
            FromDate = new DateTimeOffset(DateTimeOffset.Now.Year - 1, DateTimeOffset.Now.Month, 1,  0, 0, 0, TimeSpan.Zero).DateTime;
            ToDate = DateTimeOffset.Now.Date;
        }
        
        public string FullName { get; set; }

        public int PositionId { get; set; }

        public int Status { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IEnumerable<Position> Positions { get; set; }

        public  List<SelectListItem> StatusList { get; set; }
    }
}