// -----------------------------------------------------------------------
// <copyright file="GetTeamByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get Team By Id Criteria.
    /// </summary>
    public class GetTeamByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetGroup";
        }
    }
}
