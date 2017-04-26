namespace Fanex.NexHR.Criteria.Postnatal
{
    using Data.Repository;

    public class GetAllPostnatalCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Year { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_GetAll";
        }
    }
}
