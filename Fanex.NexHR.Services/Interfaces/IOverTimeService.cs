namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Overtime;
    using Microsoft.AspNetCore.Http;
    using Models;
    using Models.Overtime;
    using ViewModels.Overtime;

    public interface IOvertimeService
    {
        Task<IList<Overtime>> GetOvertime(GetOvertimeCriteria criteria);

        Task<Overtime> GetOvertimeById(GetOvertimeByIdCriteria criteria);

        Task<IList<EmployeeOvertime>> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria);

        Task<IList<EmployeeOvertime>> GetMyOvertime(GetMyOvertimeCriteria criteria);

        Task<IList<OvertimeSummary>> GetOvertimeSummary(OvertimeSummaryCriteria criteria);

        Task<IList<OvertimeConversion>> GetConversions(GetOvertimeConversionsCriteria criteria);
        Task<IEnumerable<StaffInfoNexHour>> GetListStaffInfoNexHourAsync(GetListStaffInfoNexHourCriteria criteria);
        Task<ValueTuple<IEnumerable<OvertimeExcelDataViewModel>, IEnumerable<int>>> GetEmployeesFromFileExcel(IFormFile file);

        Task<NexHour> GetNexHour(GetNexHourCriteria criteria);

        Task<IList<Overtime>> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria);

        Task<IEnumerable<ReportOvertimeHRDetail>> GetReportOvertimeHRDetail(ReportOvertimeHRDetailCriteria criteria);

        Task SendMailForNEXRequest(string subject, CreateOvertimeViewModel model);

        Task SendMailForApprovalNotification(string subject, int overtimeId, int createdBy, int approvedBy);

        Task SendMailForNotification(string subject, OvertimeVerifyViewModel model);

        Task SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model);
    }
}
