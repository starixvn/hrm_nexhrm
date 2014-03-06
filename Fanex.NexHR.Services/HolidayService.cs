// --------------------------------------------------------------------------
// <copyright file="HolidayService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Holiday service.
    /// </summary>
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository = null)
        {
            this._holidayRepository = holidayRepository ?? new HolidayRepository();
        }

        public int Insert(InsertNewHolidayCriteria criteria)
        {
            int result = 0;

            if (criteria != null)
            {
                result = this._holidayRepository.Insert(criteria);
            }

            return result;
        }

        /// <summary>
        /// Delete holiday.
        /// </summary>
        /// <param name="holidayId">The holiday ID.</param>
        /// <param name="modifiedBy">The ID of user that deleted holiday.</param>
        /// <returns>The boolean result.</returns>
        public bool Delete(int holidayId, int modifiedBy)
        {
            bool result = false;

            if (holidayId > 0)
            {
                var criteria = new DeleteHolidayCriteria
                {
                    Id = holidayId,
                    ModifiedBy = modifiedBy,
                    ModifiedDate = DateTime.Now
                };

                result = this._holidayRepository.Delete(criteria);
            }

            return result;
        }

        public bool Update(UpdateHolidayCriteria criteria)
        {
            bool result = false;

            if (criteria != null)
            {
                result = this._holidayRepository.Update(criteria);
            }

            return result;
        }

        public IList<Holiday> GetByYear(int year)
        {
            var criteria = new GetHolidayByYearCriteria
            {
                Year = year
            };

            return this._holidayRepository.GetByYear(criteria);
        }

        /// <summary>
        /// Get holiday by ID.
        /// </summary>
        /// <param name="id">The holiday ID.</param>
        /// <returns>The Holiday.</returns>
        public Holiday GetById(int id)
        {
            Holiday model = null;
            var criteria = new GetHolidayByIdCriteria
            {
                Id = id
            };

            if (criteria != null)
            {
                model = this._holidayRepository.GetById(criteria);
            }

            return model;
        }
    }
}