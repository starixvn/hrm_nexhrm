namespace Fanex.NexHR.Criteria.Applicant
{
    public class UpdateOnTopApplicantCommand : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid() => Id > 0;

        public override string GetSettingKey() => "Applicant_UpdateOnTop";
    }
}