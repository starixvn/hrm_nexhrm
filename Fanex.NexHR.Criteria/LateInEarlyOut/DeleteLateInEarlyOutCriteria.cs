namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    public class DeleteLateInEarlyOutCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_Delete";
        }
    }
}