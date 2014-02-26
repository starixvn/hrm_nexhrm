// -----------------------------------------------------------------------
// <copyright file="GetTeamByManagerIdCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Team By Manager Id Criteria.
    /// </summary>
    public class GetTeamByManagerIdCriteria : CriteriaBase
    {
        public int ManagerId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetTeamManagedById";
        }
    }
}
