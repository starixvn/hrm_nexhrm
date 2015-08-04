// --------------------------------------------------------------------------
// <copyright file="IRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface IRequestRepository
    {
        IList<RequestEntity> GetRequest(ICriteria criteria);

        RequestEntity GetRequestEntity(ICriteria criteria);

        decimal GetNumberRequestInMonth(ICriteria criteria);
    }
}
