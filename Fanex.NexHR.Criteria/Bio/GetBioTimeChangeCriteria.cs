namespace Fanex.NexHR.Criteria.Bio
{
    using System;
    using Data.Repository;

    public class GetBioTimeChangeCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public override string GetSettingKey() => "BioStationTimeChange_Get";

        public override bool IsValid()
        {
            if (FromDate == null)
            {
                return true;
            }

            if (ToDate == null)
            {
                return true;
            }

            if (FromDate != null && ToDate != null)
            {
                return FromDate <= ToDate;
            }

            return false;
        }
    }
}
