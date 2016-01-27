// --------------------------------------------------------------------------
// <copyright file="DetailEmployeeAttendanceModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2016.1.27">Created.</change>
// </history>

namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    /// <summary>
    /// Detail employee attendance model.
    /// </summary>
    public class DetailEmployeeAttendanceModel
    {
        public DetailEmployeeAttendanceModel()
        {
            EmployeeAttendances = new List<EmployeeAttendanceModel>();
        }

        public IList<EmployeeAttendanceModel> EmployeeAttendances { get; set; }

        public int TotalItems { get; set; }
    }
}
