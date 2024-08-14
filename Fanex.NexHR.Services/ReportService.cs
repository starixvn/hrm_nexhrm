namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Report;
    using Interfaces;
    using Models;
    using Models.Report;
    using Repositories.Interfaces;

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IBaseService baseService;

        public ReportService(IReportRepository reportRepository, IBaseService baseService)
        {
            this.reportRepository = reportRepository;
            this.baseService = baseService;
        }

        public async Task<IList<TimeRecord>> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            IList<TimeRecord> timeRecords = null;
            if (criteria != null && criteria.IsValid())
            {
                timeRecords = await reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords ?? new List<TimeRecord>();
        }

        public async Task<bool> GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria)
        {
            return await baseService.GetDataEntityAsync<bool>(criteria);
        }

        public async Task<IEnumerable<LateInEarlyOutSummary>> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria)
        {
            var lateInEarlyOuts = await baseService.GetDataAsync<LateInEarlyOutSummary>(criteria);
            return lateInEarlyOuts;
        }

        public async Task<IList<LateInEarlyOutDetail>> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria)
        {
            IList<LateInEarlyOutDetail> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = await reportRepository.GetLateInEarlyOutDetail(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDetail>();
        }

        public async Task<IList<RequestsMonitoring>> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria)
        {
            IList<RequestsMonitoring> requestsMonitorings = null;
            if (criteria != null && criteria.IsValid())
            {
                requestsMonitorings = await reportRepository.GetRequestMonitoring(criteria);
            }

            return requestsMonitorings ?? new List<RequestsMonitoring>();
        }

        public async Task<IList<TotalWorkingHours>> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria)
        {
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = await reportRepository.GetTotalWorkingHours(criteria);
            }

            return totalWorkingHours ?? new List<TotalWorkingHours>();
        }

        public async Task<IList<EmployeeAttendanceModel>> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria)
        {
            IList<EmployeeAttendanceModel> attendances = null;
            if (criteria != null && criteria.IsValid())
            {
                attendances = await reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return attendances ?? new List<EmployeeAttendanceModel>();
        }

        public async Task<IList<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = await reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDiscipline>();
        }

        public async Task<IList<WorkingHoursStatistic>> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria)
        {
            IList<WorkingHoursStatistic> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = await reportRepository.GetWorkingHoursStatistic(criteria);
            }

            return totalWorkingHours ?? new List<WorkingHoursStatistic>();
        }

        public async Task<IEnumerable<HeadCount>> GetTeamHeadCountInYear(GetHeadCountByYearCriteria criteria)
        {
            IEnumerable<HeadCount> headCount = await baseService.GetDataAsync<HeadCount>(criteria);
            return headCount;
        }

        public async Task<IList<YearlyNexRequest>> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria)
        {
            IList<YearlyNexRequest> listYearlyNexRequest = new List<YearlyNexRequest>();
            if (criteria != null && criteria.IsValid())
            {
                listYearlyNexRequest = await reportRepository.GetYearlyNexRequest(criteria);
            }

            return listYearlyNexRequest.ToList();
        }
    }
}