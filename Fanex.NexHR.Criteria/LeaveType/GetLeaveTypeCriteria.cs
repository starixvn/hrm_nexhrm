// -----------------------------------------------------------------------
// <copyright file="GetLeaveTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.LeaveType
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get leave type criteria.
    /// </summary>
    public class GetLeaveTypeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        private bool _excludeNotShown = true;

        public bool ExcludeNotShown
        {
            get
            {
                return _excludeNotShown;
            }

            set 
            {
                _excludeNotShown = value;
            }
        }

        public override string GetSettingKey()
        {
            return "LeaveType_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
