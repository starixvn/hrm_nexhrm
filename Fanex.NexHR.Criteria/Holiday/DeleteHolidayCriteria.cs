// -----------------------------------------------------------------------
// <copyright file="DeleteHolidayCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Holiday
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Delete holiday criteria.
    /// </summary>
    public class DeleteHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
