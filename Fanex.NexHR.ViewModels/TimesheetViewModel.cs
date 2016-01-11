// --------------------------------------------------------------------------
// <copyright file="TimesheetViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Tim.Trinh" date="2015.12.25">Add RequestTypes</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Resources;

    public class TimesheetViewModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int? EmpId { get; set; }

        [Display(Name = "Request Type")]
        public short RequestType { get; set; }

        public List<SelectListItem> RequestTypes
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = Labels.All, Value = "0"},
                    new SelectListItem() {Text = Labels.CheckIn, Value = "1"},
                    new SelectListItem() {Text = Labels.CheckOut, Value = "2"},
                    new SelectListItem() {Text = Labels.LateIn, Value = "3"},
                    new SelectListItem() {Text = Labels.EarlyOut, Value = "4"},
                    new SelectListItem() {Text = Labels.NotWorking, Value = "5"}
                };
            }
        }
    }
}