// --------------------------------------------------------------------------
// <copyright file="Team.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// Team model.
    /// </summary>
    public class Team : EntityBase<int>
    {
        public Department Department { get; set; }

        public Team Parent { get; set; }

        public Employee Leader { get; set; }

        public Employee AssistantLeader { get; set; }

        public int Rank { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifer { get; set; }
    }
}
