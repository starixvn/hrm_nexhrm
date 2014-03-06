// --------------------------------------------------------------------------
// <copyright file="IHolidayRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;

    public interface IHolidayRepository
    {
        int Insert(InsertNewHolidayCriteria criteria);

        bool Delete(DeleteHolidayCriteria criteria);

        bool Update(UpdateHolidayCriteria criteria);

        IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria);

        Holiday GetById(GetHolidayByIdCriteria criteria);
    }
}