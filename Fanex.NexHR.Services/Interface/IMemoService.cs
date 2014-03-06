// --------------------------------------------------------------------------
// <copyright file="IMemoService.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;

    public interface IMemoService
    {
        bool Delete(ICriteria criteria);

        IList<Memo> GetAll(ICriteria criteria);

        Memo GetById(ICriteria criteria);

        int Insert(InsertMemoCriteria criteria);

        bool Update(UpdateMemoCriteria criteria);
    }
}
