namespace Fanex.NexHR.Criteria.Employee
{
    using Data;
    using Data.Repository;

    public class GetEmployeeToeicScoreCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        private int _pageNumber = 1;

        [SpParam(Ignored = true)]
        private int _pageSize = int.MaxValue;

        [SpParam(Ignored = true)]
        private int _totalRow = 2000;

        public string Query { get; set; }

        public int? GroupId { get; set; }

        public int? TitleId { get; set; }

        public int? Score { get; set; }

        public bool? Enable { get; set; }

        public bool? IsDelete { get; set; }

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }

            set
            {
                _pageNumber = value;
            }
        }

        public int PageSize 
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = value;
            }
        }

        public int TotalRow 
        {
            get
            {
                return _totalRow;
            }

            set
            {
                _totalRow = value;
            }
        }

        public override string GetSettingKey()
        {
            return "TOEIC_GetAllScores";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
