// -----------------------------------------------------------------------
// <copyright file="UpdateOffDayForAllEmployeesCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update off day for all employees criteria.
    /// </summary>
    public class UpdateOffDayForAllEmployeesCriteria : CriteriaBase
    { 
        public DateTime Date { get; set; }

        public int IsOffDay { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_SetOffDayForAllEmployees";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
