// --------------------------------------------------------------------------
// <copyright file="GetAllEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.7">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get All Employee Criteria.
    /// </summary>
    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRow { get; set; }

        public int? GroupId { get; set; }

        public int? Enable { get; set; }

        public int? IsDelete { get; set; }

        public int? TitleId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetEmployees";
        }
    }
}
