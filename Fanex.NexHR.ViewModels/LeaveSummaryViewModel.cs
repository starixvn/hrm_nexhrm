// --------------------------------------------------------------------------
// <copyright file="LeaveSummaryViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public string Team { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }

        public IList<Team> TeamCollection { get; set; }

        public ShowOpenAndCloseAnnualBalance ShowOpenAndCloseAnnualBalance { get; set; }

        public IList<SelectListItem> FilteringMinAnnualBalances
        {
            get
            {
                IList<SelectListItem> filteringMinAnnualBalances = new List<SelectListItem>();

                for (int i = 1; i <= 10; i++)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    };

                    filteringMinAnnualBalances.Add(item);
                }

                return filteringMinAnnualBalances;
            }
        }
    }
}