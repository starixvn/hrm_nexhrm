// --------------------------------------------------------------------------
// <copyright file="Team.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    public class Team : EntityBase<int>
    {
        public int TeamId { get; set; }

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
