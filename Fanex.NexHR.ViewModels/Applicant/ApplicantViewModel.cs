namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ApplicantViewModel
    {
        private const int months = 6; 

        public ApplicantViewModel()
        {
            FromDate = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month - months, 1,  0, 0, 0, TimeSpan.Zero).DateTime;
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