// --------------------------------------------------------------------------
// <copyright file="ITOEICRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ITOEICRepository
    {
        TOEIC GetById(int empId);

        int Update(TOEIC model);

        int NewUpdate(TOEIC model);
    }
}
