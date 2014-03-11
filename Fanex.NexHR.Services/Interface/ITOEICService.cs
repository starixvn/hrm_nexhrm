// --------------------------------------------------------------------------
// <copyright file="ITOEICService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.TOEIC;
    using Fanex.NexHR.Models;

    public interface ITOEICService
    {
        TOEIC GetById(GetTOEICScoreByEmployeeIdCriteria criteria);

        int UpdateFullToeicScore(UpdateFullTOEICScoreCriteria criteria);

        int UpdateToeicScore(UpdateTOEICScoreCriteria criteria);
    }
}
