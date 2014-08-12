// --------------------------------------------------------------------------
// <copyright file="SystemReminderModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.12">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// System Reminder Model.
    /// </summary>
    public class SystemReminderModel
    {
        public List<Employee> EmployeeHasBirthday { get; set; }

        public List<Employee> EmployeeHasFirstWorkingDay { get; set; }
    }
}
