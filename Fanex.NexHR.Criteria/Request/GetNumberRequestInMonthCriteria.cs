// --------------------------------------------------------------------------
// <copyright file="GetRequestByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Number Request In Month Criteria.
    /// </summary>
    public class GetNumberRequestInMonthCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetNumberRequestInMonth";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
