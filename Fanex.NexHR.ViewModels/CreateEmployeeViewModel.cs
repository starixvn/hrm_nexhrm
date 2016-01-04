// --------------------------------------------------------------------------
// <copyright file="CreateEmployeeViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Tim.Trinh" date="2015.01.04">Add Teams, Shifts</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public class CreateEmployeeViewModel
    {
        public CreateEmployeeViewModel()
        {
            Teams = new List<Team>();
            Shifts = new List<SelectListItem>();
        }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string DomainUsername { get; set; }

        [Required]
        public string Code { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        [Required]
        public int TeamId { get; set; }

        public int ProcessBy { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<SelectListItem> Shifts { get; set; }
    }
}