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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
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
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public int Insert(Holiday model)
        {
            using (var db = new ObjectDb("Holiday_Insert"))
            {
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return (int)db.ExecuteScalar();
            }
        }

        /// <summary>
        /// Delete holiday.
        /// </summary>
        /// <param name="id">The holiday ID.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <returns>The result.</returns>
        public bool Delete(int id, int modifiedBy)
        {
            using (var db = new ObjectDb("Holiday_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                db.AddInParameter("@ModifiedBy", DbType.Int16, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Update holiday.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public bool Update(Holiday model)
        {
            using (ObjectDb db = new ObjectDb("Holiday_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Get holiday by year.
        /// </summary>
        /// <param name="year">The year value.</param>
        /// <returns>List of holiday.</returns>
        public IList<Holiday> GetByYear(int year)
        {
            using (var db = new ObjectDb("Holiday_GetByYear"))
            {
                return db.Query<Holiday>(new { year }).ToList();
            }
        }

        /// <summary>
        /// Get holiday by ID.
        /// </summary>
        /// <param name="id">The holiday ID.</param>
        /// <returns>The holiday object.</returns>
        public Holiday GetById(int id)
        {
            using (var db = new ObjectDb("Holiday_GetById"))
            {
                return db.QueryEntity<Holiday>(new { id });
            }
        }
    }
}
