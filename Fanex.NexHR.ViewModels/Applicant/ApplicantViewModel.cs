namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ApplicantViewModel
    {

        public ApplicantViewModel()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Now;
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