// --------------------------------------------------------------------------
// <copyright file="ITOEICService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;

    public interface ITOEICService
    {
        TOEIC GetById(ICriteria criteria);

        int Update(TOEIC model);

        int NewUpdate(TOEIC model);
    }
}
