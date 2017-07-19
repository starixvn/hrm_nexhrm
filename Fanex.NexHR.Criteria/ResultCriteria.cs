namespace Fanex.NexHR.Criteria
{
    using Data.Repository;

    public class ResultCriteria : CriteriaBase
    {
        public ResultCriteria()
        {
            ErrCode = 0;
            ErrMsg = string.Empty;
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}
