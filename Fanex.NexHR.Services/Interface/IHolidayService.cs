// --------------------------------------------------------------------------
// <copyright file="IHolidayService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;

    public interface IHolidayService
    {
        bool Delete(int holidayId, int modifiedBy);

        Holiday GetById(int id);

        IList<Holiday> GetByYear(int year);

        int Insert(InsertNewHolidayCriteria criteria);

        bool Update(UpdateHolidayCriteria criteria);
    }
}