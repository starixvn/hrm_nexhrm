namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Common;
    using Core.Enums;
    using Models;
    using Resources;

    public class RequestViewModel : PagerOptionModel
    {
        public RequestViewModel()
        {
            StatusId = (byte)RequestStatuse.Pending;
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            DateOption = 1;
            TeamId = -1;
            EmployeeId = -1;
        }

        public int DateOption { get; set; }

        [Display(Name = "Status")]
        public byte StatusId { get; set; }

        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int RequestType { get; set; }

        public bool IsFilterByEffectedDate { get; set; }

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }

        public IList<LeaveType> LeaveTypes { get; set; }

        public List<SelectListItem> DateTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.EffectedDate, Value = "true" },
            new SelectListItem { Text = Labels.CreatedDate, Value = "false" },
        };

        public List<SelectListItem> RequestTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.All, Value = "5" },
            new SelectListItem { Text = Labels.Leave, Value = "1" },
            new SelectListItem { Text = Labels.CheckInOut, Value = "4" }
        };

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

            if (Teams == null)
            {
                Teams = new List<Team>();
            }

            foreach (Team team in Teams)
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