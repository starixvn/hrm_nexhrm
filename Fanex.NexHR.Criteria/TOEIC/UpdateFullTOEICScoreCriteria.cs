// -----------------------------------------------------------------------
// <copyright file="UpdateFullTOEICScoreCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.TOEIC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Update Full TOEIC Score Criteria.
    /// </summary>
    public class UpdateFullTOEICScoreCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int? Listening { get; set; }

        public int? Reading { get; set; }

        public DateTime? LRExamDate { get; set; }

        public int? OldListening { get; set; }

        public int? OldReading { get; set; }

        public int? Speaking { get; set; }

        public int? Writing { get; set; }

        public DateTime? SWExamDate { get; set; }

        public int? OldSpeaking { get; set; }

        public int? OldWriting { get; set; }

        public DateTime? InputDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "TOEIC_Update";
        }
    }
}
