// --------------------------------------------------------------------------
// <copyright file="RequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Request view model.
    /// </summary>
    public class RequestViewModel
    {
        public RequestViewModel()
        {
            this.StatusId = (byte)RequestStatuse.Pending;
            this.FromDate = DateTime.Now;
            this.ToDate = DateTime.Now;
            this.DateOption = 1;
            this.TeamId = -1;
            this.EmployeeId = -1;
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
        public IList<Team> TeamCollection { get; set; }

        /// <summary>
        /// Get select list item team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <returns>List of select list item.</returns>
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

            if (this.TeamCollection == null)
            {
                this.TeamCollection = new List<Team>();
            }

            foreach (Team team in this.TeamCollection)
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

        public List<LeaveType> FilteringLeaveTypes { get; set; }
        

        public List<SelectListItem> FilteringDateTypes
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = Labels.EffectedDate, Value = "true"},
                    new SelectListItem() {Text = Labels.CreatedDate, Value = "false"},
                };
            }
        }

        public List<SelectListItem> FilteringRequestTypes
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = Labels.All, Value = "5"},
                    new SelectListItem() {Text = Labels.Leave, Value = "1"},
                    new SelectListItem() {Text = Labels.CheckInOut, Value = "4"}
                };
            }
        }
    }
}