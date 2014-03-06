// --------------------------------------------------------------------------
// <copyright file="GetEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.6">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Employee Criteria.
    /// </summary>
    public class GetEmployeeCriteria : CriteriaBase
    {
        public int? Id { get; set; }

        public string DomainUserName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetEmployee";
        }
    }
}
