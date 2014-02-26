// -----------------------------------------------------------------------
// <copyright file="DeleteTeamCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.NAP.Framework.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Delete Team Criteria.
    /// </summary>
    public class DeleteTeamCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Delete";
        }
    }
}
