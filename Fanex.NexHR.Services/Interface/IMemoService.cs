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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IMemoService
    {
        bool Delete(int id);

        IList<Memo> GetAll(MemoViewModel model);

        Memo GetById(int id);

        int Insert(Memo entity);

        bool Update(Memo entity);
    }
}
