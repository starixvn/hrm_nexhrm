// --------------------------------------------------------------------------
// <copyright file="RequestsMonitoring.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class RequestsMonitoring
    {
        public Employee Requester { get; set; }

        public int TotalCheckInOut { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int TotalChangeShift { get; set; }

        public int TotalLeave { get; set; }
    }
}
