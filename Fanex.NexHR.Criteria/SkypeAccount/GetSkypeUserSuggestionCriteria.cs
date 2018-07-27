namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Data.Repository;

    public class GetSkypeUserSuggestionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SkypeAccount_GetSuggestion";

        public override bool IsValid() => true;
    }
}