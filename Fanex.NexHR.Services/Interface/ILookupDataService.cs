// --------------------------------------------------------------------------
// <copyright file="ILookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        LookupData Get(string lookupCode, int lookupId);

        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);

        IList<LookupData> GetJobTitle(GetLookupDataJobTitleCriteria criteria);

        bool Delete(DeleteLookupDataCriteria criteria);

        bool Insert(InsertLookupDataCriteria criteria);

        bool Update(UpdateLookupDataCriteria criteria);
    }
}