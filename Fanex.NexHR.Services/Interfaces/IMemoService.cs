// --------------------------------------------------------------------------
// <copyright file="IMemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IMemoService
    {
        IList<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);

        void SendMail(string subject, string memoId);
    }
}
