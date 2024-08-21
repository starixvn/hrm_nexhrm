namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Data;
    using Models.AnnualLeave;

    public class ResetAnnualLeavesCommand : BaseCommand
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
                    xmlTemplate.Append(string.Format(XmlTemplate, item.EmpId, item.LeaveBalance));
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
        public IEnumerable<LeaveSummary> LeaveSummaries { get; set; }

        public override string GetSettingKey() => "AnnualLeave_ResetMultipleTransactions";

        public override bool IsValid() => LeaveSummaries != null && LeaveSummaries.Any();

    }
}