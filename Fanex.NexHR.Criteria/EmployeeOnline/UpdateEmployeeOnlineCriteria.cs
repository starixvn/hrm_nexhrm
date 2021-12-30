namespace Fanex.NexHR.Criteria.EmployeeOnlines
{
    using System.Collections.Generic;
    using Data.Repository;
    using Fanex.NexHR.Models.EmployeeOnline;
    using System.Linq;
    using Fanex.NexHR.Core.XML;
    using Fanex.Data;

    public class UpdateEmployeeOnlineCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public IList<EmployeeOnline> EmployeeOnlines { get; set; }

        public override string GetSettingKey() => "EmployeeOnline_UpdateStatus";

        public string EmployeeOnlineXML 
        { 
            get
            {
                var employeeOnlines = EmployeeOnlines.Select(x => new EmployeeOnlineXML
                {
                    EmpId = x.EmpId,
                    CheckingTime = x.CheckingTime,
                    IsOnline = x.IsOnline,
                    PCName = x.PCName
                });

                return employeeOnlines.ToXML().ToString();
            }   
        }

        public override bool IsValid() => true;
    }
}
