// --------------------------------------------------------------------------
// <copyright file="TOEICRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Data;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interface;

    /// <summary>
    /// TOEIC repository.
    /// </summary>
    public class TOEICRepository : ITOEICRepository
    {
        /// <summary>
        /// Get TOEIC of specific employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>TOEIC object.</returns>
        public TOEIC GetById(ICriteria criteria)
        {
            TOEIC toeic = new TOEIC();
            if (criteria.IsValid())
            {
                using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
                {
                    toeic = db.QueryEntity<TOEIC>(criteria.GetSpParams());
                }
            }

            return toeic;
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="criteria">Update TOEIC Score Criteria.</param>
        /// <returns>Effected row.</returns>
        public int Update(ICriteria criteria)
        {
            int updated = 0;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                updated = db.ExecuteNonQuery(criteria.GetSpParams()) == -1 ? 1 : 0;
            }

            return updated;
        }
    }
}
