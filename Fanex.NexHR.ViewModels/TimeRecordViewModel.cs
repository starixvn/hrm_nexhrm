// --------------------------------------------------------------------------
// <copyright file="TimeRecordViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
//   <change who="Tim.Trinh" date="2015.12.25">Add FilteringDateRanges</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// TimeRecord ViewModel.
    /// </summary>
    public class TimeRecordViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public IList<Team> Teams { get; set; }

        public byte? IsLocal { get; set; }

        public int DateOption { get; set; }

        public List<SelectListItem> FilteringDateRanges
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = Labels.DateRange, Value = "0"},
                    new SelectListItem() {Text = Labels.ToDay, Value = "1"},
                    new SelectListItem() {Text = Labels.Yesterday, Value = "2"},
                    new SelectListItem() {Text = Labels.CurrentWeek, Value = "3"},
                    new SelectListItem() {Text = Labels.LastWeek, Value = "4"},
                    new SelectListItem() {Text = Labels.CurrentMonth, Value = "5"},
                    new SelectListItem() {Text = Labels.LastMonth, Value = "6"},
                    new SelectListItem() {Text = Labels.SinceLastMonth, Value = "7"}
                };
            }
        }
    }
}