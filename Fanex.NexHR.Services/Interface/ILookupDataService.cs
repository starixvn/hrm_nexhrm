// --------------------------------------------------------------------------
// <copyright file="ILookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        LookupData Get(string lookupCode, int lookupId);

        IList<LookupData> GetAll(ICriteria criteria);

        IList<LookupData> GetJobTitle(ICriteria criteria);

        bool Delete(ICriteria criteria);

        bool Insert(ICriteria criteria);

        bool Update(ICriteria criteria);
    }
}