namespace Fanex.NexHR.Criteria.HistoryWork 
{
    using Fanex.Data.Repository;
    using System;

    public class FilterHistoryWorkCriteria : CriteriaBase
    {     
        public int EmpId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime ToDate { get; set; }

   
        public override string GetSettingKey()
        {
            return "HistoryWork_Filter";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
