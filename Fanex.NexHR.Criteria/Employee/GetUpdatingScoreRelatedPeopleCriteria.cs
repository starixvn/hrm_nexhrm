// --------------------------------------------------------------------------
// <copyright file="GetUpdatingScoreRelatedPeopleCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Aaron.Cao" date="2015.03.16">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get related people in updating TOEIC score criteria.
    /// </summary>
    public class GetUpdatingScoreRelatedPeopleCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "TOEIC_GetUpdatingScoreRelatedPeople";
        }
    }
}
