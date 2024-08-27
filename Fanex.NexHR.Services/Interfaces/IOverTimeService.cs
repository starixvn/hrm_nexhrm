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
        Task<IEnumerable<Overtime>> GetOvertime(GetOvertimeCriteria criteria);

        Task<Overtime> GetOvertimeById(GetOvertimeByIdCriteria criteria);

        Task<ValueTuple<IEnumerable<OvertimeExcelDataViewModel>, IList<int>>> GetEmployeesFromFileExcel(IFormFile file);

        Task<IEnumerable<Overtime>> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria);

        Task SendMailForNEXRequest(string subject, CreateOvertimeViewModel model);

        Task SendMailForApprovalNotification(string subject, int overtimeId, int createdBy, int approvedBy);

        Task SendMailForNotification(string subject, OvertimeVerifyViewModel model);

        Task SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model);
    }
}
