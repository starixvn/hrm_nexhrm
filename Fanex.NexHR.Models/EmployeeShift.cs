// --------------------------------------------------------------------------
// <copyright file="EmployeeShift.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Employee shift.
    /// </summary>
    public class EmployeeShift
    {
        public int EmpId { get; set; }

        public int? MonShiftId { get; set; }

        public int? TueShiftId { get; set; }

        public int? WedShiftId { get; set; }

        public int? ThuShiftId { get; set; }

        public int? FriShiftId { get; set; }

        public int? SatShiftId { get; set; }

        public int? SunShiftId { get; set; }

        public string MonShiftName { get; set; }

        public string TueShiftName { get; set; }

        public string WedShiftName { get; set; }

        public string ThuShiftName { get; set; }

        public string FriShiftName { get; set; }

        public string SatShiftName { get; set; }

        public string SunShiftName { get; set; }
    }
}
