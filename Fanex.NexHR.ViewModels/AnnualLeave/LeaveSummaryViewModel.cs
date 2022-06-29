namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.AnnualLeave;

    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }

        public IList<Team> Teams { get; set; }

        public ShowOpenAndCloseAnnualBalance ShowOpenAndCloseAnnualBalance { get; set; }

        public IList<SelectListItem> MinAnnualBalances
        {
            get
            {
                IList<SelectListItem> minAnnualBalances = new List<SelectListItem>();
                const byte annualBalanceLength = 10;

                for (int i = 1; i <= annualBalanceLength; i++)
                {
                    SelectListItem item = new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    };

                    minAnnualBalances.Add(item);
                }

                return minAnnualBalances;
            }
        }
    }
}