namespace Fanex.NexHR.Criteria.Applicant
{
    public class DeleteApplicantCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "Applicant_Delete";
        }
    }
}