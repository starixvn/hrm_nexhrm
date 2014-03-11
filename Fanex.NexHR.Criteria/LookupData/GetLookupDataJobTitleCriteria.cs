// --------------------------------------------------------------------------
// <copyright file="GetLookupDataJobTitleCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get LookupData Job Title Criteria.
    /// </summary>
    public class GetLookupDataJobTitleCriteria : CriteriaBase
    {
        public string JobTitle { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Lookup_GetJobTitle";
        }
    }
}
