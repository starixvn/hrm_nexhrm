// --------------------------------------------------------------------------
// <copyright file="LeaveTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.10">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Leave type repository.
    /// </summary>
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        public IList<LeaveType> GetAll(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                return db.Query<LeaveType>(criteria.GetSpParams()).ToList();
            }
        }
    }
}
