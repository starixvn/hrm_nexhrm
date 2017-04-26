// --------------------------------------------------------------------------
// <copyright file="CheckInOutInfo.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.8">Created</change>
// </history>

namespace AutoJobs
{
    using System;

    /// <summary>
    /// Check in/out information.
    /// </summary>
    [Serializable]
    public class CheckInOutInfo
    {
        public string Employee { get; set; }

        public string Date { get; set; }

        public string InOutType { get; set; }

        public string Shift { get; set; }

        public string Remark { get; set; }
    }
}
