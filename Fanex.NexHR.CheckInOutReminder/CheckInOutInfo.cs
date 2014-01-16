// --------------------------------------------------------------------------
// <copyright file="CheckInOutInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.CheckInOutReminder
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [Serializable]
    public class CheckInOutInfo
    {
        public string Employee { get; set; }

        public string Date { get; set; }

        public string Type { get; set; }

        public string Shift { get; set; }

        public string Remark { get; set; }
    }
}
