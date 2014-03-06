// --------------------------------------------------------------------------
// <copyright file="GetAllMemoCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get All Memo Criteria.
    /// </summary>
    public class GetAllMemoCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public int Category { get; set; }

        public string Content { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_GetAll";
        }
    }
}
