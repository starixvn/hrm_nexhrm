using System;
using Fanex.Data.Repository;

namespace BIO.WebAPI.Criteria
{
    public class GetRedmineTimeEntriesCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override string GetSettingKey() => "API_Redmine_GetTimeEntries";

        public override bool IsValid() => true;
    }
}