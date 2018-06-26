namespace Fanex.NexHR.Criteria.Bio
{
    using System;
    using Data.Repository;

    public class GetBioTimeChangeCriteria : CriteriaBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public override string GetSettingKey() => "BioStationTimeChange_Get";

        public override bool IsValid()
        {
            if (From == null)
            {
                return true;
            }

            if (To == null)
            {
                return true;
            }

            if (From != null && To != null)
            {
                return From <= To;
            }

            return false;
        }
    }
}
