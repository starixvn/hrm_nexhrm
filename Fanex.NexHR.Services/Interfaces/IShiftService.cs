// --------------------------------------------------------------------------
// <copyright file="IShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll(GetAllShiftCriteria criteria);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        IList<SelectListItem> GetSelectList();

        Shift GetByDate(GetShiftByDateCriteria criteria);
    }
}
