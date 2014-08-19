// -----------------------------------------------------------------------
// <copyright file="DepartmentViewModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.19">Created.</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// The view model of Department.
    /// </summary>
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Parent", ResourceType = typeof(Labels))]
        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        [Display(Name = "Manager", ResourceType = typeof(Labels))]
        public string ManagerName { get; set; }
    }
}
