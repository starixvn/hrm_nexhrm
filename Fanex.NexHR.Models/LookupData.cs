// --------------------------------------------------------------------------
// <copyright file="LookupData.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Lookup data.
    /// </summary>
    public class LookupData
    {
        public int Id { get; set; }

        public int LookupId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
