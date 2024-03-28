namespace Fanex.NexHR.Criteria.Applicant
{
    public class UpdateOnTopApplicantCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "Applicant_UpdateOnTop";
        }
    }
}