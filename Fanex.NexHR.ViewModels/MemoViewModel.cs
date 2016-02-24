// --------------------------------------------------------------------------
// <copyright file="MemoViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using System.Web.Mvc;
    using Fanex.NexHR.Resources;

    public class MemoViewModel
    {
        public string Content { get; set; }

        public int Year { get; set; }

        public int Category { get; set; }

        public bool Status { get; set; }

        public IList<LookupData> MemoCategories { get; set; }

        public List<SelectListItem> StatusTypes
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() { Text = Labels.All, Value = "-1" },       
                    new SelectListItem() { Text = Labels.Active, Value = "1" },                    
                    new SelectListItem() { Text = Labels.Deactive, Value = "0" }
                };
            }
        }
    }
}