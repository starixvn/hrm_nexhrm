namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Data;
    using Data.Repository;
    using Models.AnnualLeave;

    public class ResetAnnualLeavesCriteria : CriteriaBase
    {
        private const string XmlTemplate = "<Employee Id='{0}' LeaveBalance='{1}' />";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        [SpParam(SqlParamName = "EmpValue")]
        public string EmployeeValue
        {
            get
            {
                var xmlTemplate = new StringBuilder(BeginRootTag);

                foreach (var item in LeaveSummaries)
                {
                    xmlTemplate.Append(string.Format(XmlTemplate, item.Employee.Id, item.LeaveBalance));
                }

                xmlTemplate.Append(EndRootTag);

                return xmlTemplate.ToString();
            }
        }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal ResetDays { get; set; }

        public int CreatedBy { get; set; }

        [SpParam(Ignored = true)]
        public IList<LeaveSummary> LeaveSummaries { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_ResetMultipleTransactions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}