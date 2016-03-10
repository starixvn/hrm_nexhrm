namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.ViewModels.Overtime;
    using Models;
    using Models.Overtime;

    public interface IOvertimeService
    {
        IList<Overtime> GetOvertime(GetOvertimeCriteria criteria);

        Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria);

        IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria);

        List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria);

        void SendMail(string subject, CreateOvertimeViewModel model);
    }
}
