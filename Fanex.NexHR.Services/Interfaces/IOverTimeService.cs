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

        IList<Overtime> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria);

        IList<ReportOvertimeHRDetail> GetReportOvertimeHRDetail(ReportOvertimeHRDetailCriteria criteria);

        void SendMailForNEXRequest(string subject, CreateOvertimeViewModel model);

        void SendMailForApprovalNotification(string subject, int createdBy, int approvedBy);

        void SendMailForNotification(string subject, OvertimeVerifyViewModel model);

        void SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model);
    }
}
