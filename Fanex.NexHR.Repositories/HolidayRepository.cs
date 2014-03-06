// --------------------------------------------------------------------------
// <copyright file="HolidayRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Holiday repository.
    /// </summary>
    public class HolidayRepository : IHolidayRepository
    {
        /// <summary>
        /// Insert holiday.
        /// </summary>
        /// <param name="criteria">The criteria object.</param>
        /// <returns>The result.</returns>
        public int Insert(InsertNewHolidayCriteria criteria)
        {
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                return (int)db.ExecuteScalar(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Delete holiday.
        /// </summary>
        /// <param name="criteria">The criteria object.</param>
        /// <returns>The result.</returns>
        public bool Delete(DeleteHolidayCriteria criteria)
        {
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Update holiday.
        /// </summary>
        /// <param name="criteria">The criteria object.</param>
        /// <returns>The result.</returns>
        public bool Update(UpdateHolidayCriteria criteria)
        {
            var settingKey = criteria.GetSettingKey();

            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Get holiday by year.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of holiday.</returns>
        public IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria)
        {
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                return db.Query<Holiday>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Get holiday by ID.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The holiday object.</returns>
        public Holiday GetById(GetHolidayByIdCriteria criteria)
        {
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                return db.QueryEntity<Holiday>(criteria.GetSpParams());
            }
        }
    }
}
