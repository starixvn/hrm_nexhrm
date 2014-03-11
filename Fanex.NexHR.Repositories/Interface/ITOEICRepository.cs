// --------------------------------------------------------------------------
// <copyright file="ITOEICRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Repositories.Interface
{
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;

    public interface ITOEICRepository
    {
        TOEIC GetById(ICriteria criteria);

        int Update(ICriteria criteria);
    }
}
