// -----------------------------------------------------------------------
// <copyright file="AddNewTeamCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Add New Team Criteria.
    /// </summary>
    public class AddNewTeamCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public int AssistantLeaderId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }
    }
}
