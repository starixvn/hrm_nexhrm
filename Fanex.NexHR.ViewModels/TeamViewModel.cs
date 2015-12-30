// --------------------------------------------------------------------------
// <copyright file="TeamViewModel.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class TeamViewModel
    {
        public TeamViewModel()
        {
            FilteringDepartments = new List<DepartmentModel>();
            FilteringGroups = new List<Team>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Code", ResourceType = typeof(Labels))]
        public string TeamCode { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Labels))]
        public string TeamName { get; set; }

        [Required]
        [Display(Name = "Leader", ResourceType = typeof(Labels))]
        public string Leader { get; set; }

        public int LeaderId { get; set; }

        [Display(Name = "Assistant Leader")]
        public string AssistantLeader { get; set; }

        public int? AssistantLeaderId { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Parent Team")]
        public int? ParentId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public IList<DepartmentModel> FilteringDepartments { get; set; }

        public IList<Team> FilteringGroups { get; set; }
    }
}