// --------------------------------------------------------------------------
// <copyright file="UpdateEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.7">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Employee Criteria.
    /// </summary>
    public class UpdateEmployeeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string EmployeeCard { get; set; }

        [SpParam(SqlParamName = "dob")]
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public int? JobTitleId { get; set; }

        public bool Enabled { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public int MainGroupId { get; set; }

        public string SubGroups { get; set; }

        public string HandleRequestGroupIds { get; set; }

        public string Roles { get; set; }

        public DateTime? JoinedDate { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public bool Mariage { get; set; }

        public string Address { get; set; }

        public string CurrAddress { get; set; }

        public string IdCardNo { get; set; }

        public DateTime? IssuedDate { get; set; }

        public string IssuedPlace { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string Geomaplatitude { get; set; }

        public string Geomaplongtitude { get; set; }

        public string Distancetocompany { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_UpdateEmployee";
        }
    }
}
