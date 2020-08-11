namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Overtime;
    using Models;
    using Models.Overtime;
    using ViewModels.Overtime;

    public interface IOvertimeService
    {
        IList<Overtime> GetOvertime(GetOvertimeCriteria criteria);

        Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria);

        IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria);

        List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria);

        IList<OvertimeConversion> GetConversions(GetOvertimeConversionsCriteria criteria);

        NexHour GetNexHour(GetNexHourCriteria criteria);

        IList<ReportOvertimeHR> GetReportOvertimeHR(GetReportOvertimeHRCriteria criteria);

        void SendMailForNEXRequest(string subject, CreateOvertimeViewModel model);

        void SendMailForNotification(string subject, UpdateNexHour model);

        void SendMailForApprovalNotification(string subject, int createdBy, int approvedBy);

        void SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model);
    }
}
