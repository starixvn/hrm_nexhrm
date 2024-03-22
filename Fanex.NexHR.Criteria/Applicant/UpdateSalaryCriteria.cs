using Microsoft.AspNetCore.Http.HttpResults;

namespace Fanex.NexHR.Criteria.Applicant
{
    public class UpdateSalaryCriteria : BaseCommand
    {
        public int Id { get; set; }
        public string ExpectedSalary { get; set; }

        public string OfferedSalary { get; set; }

        public string FinalComment { get; set; }

        public string CvName { get; set; }

        public override bool IsValid() => true;
        
        public override string GetSettingKey() => "Applicant_UpdateSalary";

    }
}