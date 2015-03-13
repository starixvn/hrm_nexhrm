// -----------------------------------------------------------------------
// <copyright file="EmployeeBirthdayViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Aaron.Cao" date="2015.3.13">Apply StyleCop</change>
// </history>
namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models; 

    /// <summary>
    /// Employee's Birthday View Model.
    /// </summary>
    public class EmployeeBirthdayViewModel
    {
        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "DoB")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Display(Name = "Nick")]
        public string NickName { get; set; }
    }
}
