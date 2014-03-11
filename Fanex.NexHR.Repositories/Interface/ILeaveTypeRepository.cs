// --------------------------------------------------------------------------
// <copyright file="ILeaveTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.10">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeRepository
    {
        IList<LeaveType> GetAll(ICriteria criteria);
    }
}
