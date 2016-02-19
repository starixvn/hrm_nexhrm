namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class TeamRequestViewModel
    {
        public int DateRange { get; set; }

        public byte StatusId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        public int TeamId { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public IList<SelectListItem> GetSelectListItemTeam(int? teamId)
        {
            IList<SelectListItem> selecListItemTeam = new List<SelectListItem>();

            var firstItem = new SelectListItem
            {
                Value = string.Empty,
                Text = Labels.All,
                Selected = true
            };

            selecListItemTeam.Add(firstItem);

            if (this.Teams == null)
            {
                this.Teams = new List<Team>();
            }

            foreach (Team team in this.Teams)
            {
                var item = new SelectListItem
                {
                    Value = team.Id.ToString(),
                    Text = team.Name,
                    Selected = teamId == team.Id
                };

                selecListItemTeam.Add(item);
            }

            return selecListItemTeam;
        }
    }
}
