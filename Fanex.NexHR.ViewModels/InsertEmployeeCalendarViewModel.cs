// --------------------------------------------------------------------------
// <copyright file="InsertEmployeeCalendarViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.12.30">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;

    /// <summary>
    /// Insert Employee Calendar View Model.
    /// </summary>
    public class InsertEmployeeCalendarViewModel
    {
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
