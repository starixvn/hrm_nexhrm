// --------------------------------------------------------------------------
// <copyright file="DeleteLookupDataCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.6">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.LookupData
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Delete Lookup Data Criteria.
    /// </summary>
    public class DeleteLookupDataCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LookupData_Delete";
        }
    }
}
