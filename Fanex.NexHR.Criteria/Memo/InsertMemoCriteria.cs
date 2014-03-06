// --------------------------------------------------------------------------
// <copyright file="InsertMemoCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert Memo Criteria.
    /// </summary>
    public class InsertMemoCriteria : CriteriaBase
    {
        public string RefNo { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public string SentTo { get; set; }

        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        public int Type { get; set; }

        [SpParam(Ignored = true)]
        public bool SendMail { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_Insert";
        }
    }
}
