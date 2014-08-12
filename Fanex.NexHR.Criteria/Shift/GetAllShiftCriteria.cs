// --------------------------------------------------------------------------
// <copyright file="GetAllShiftCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get All Shift Criteria.
    /// </summary>
    public class GetAllShiftCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Shift_GetShifts";
        }
    }
}
