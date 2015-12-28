// --------------------------------------------------------------------------
// <copyright file="RequestsMonitoringViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Request monitoring view model.
    /// </summary>
    public class RequestsMonitoringViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Total Request >=")]
        public int MaxRequest { get; set; }

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