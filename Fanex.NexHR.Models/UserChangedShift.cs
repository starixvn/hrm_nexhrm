// --------------------------------------------------------------------------
// <copyright file="UserChangedShift.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// User changed shift.
    /// </summary>
    public class UserChangedShift
    {
        public int Id { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int Day { get; set; }

        public string Code { get; set; }
    }
}
