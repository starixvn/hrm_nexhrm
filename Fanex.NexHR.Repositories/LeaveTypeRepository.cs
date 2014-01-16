// --------------------------------------------------------------------------
// <copyright file="LeaveTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        public IList<LeaveType> GetAll(bool excludeNotShown = true)
        {
            using (var db = new ObjectDb("LeaveType_GetAll"))
            {
                return db.Query<LeaveType>(new { excludeNotShown }).ToList();
            }
        }
    }
}
