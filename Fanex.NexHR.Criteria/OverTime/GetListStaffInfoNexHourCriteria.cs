namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.XML;
    using Fanex.NexHR.ViewModels.Overtime;

    public class GetListStaffInfoNexHourCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public IEnumerable<OvertimeExcelDataViewModel> OvertimeExcelData { get; set; }
        public string Employees => XMLIO.ToXML(OvertimeExcelData, "ROOT").ToString();

        public override string GetSettingKey()
        {
            return "Employee_GetListStaffInfoNexHour";
        }

        public override bool IsValid() => true;
    }
}