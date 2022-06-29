namespace Fanex.NexHR.Services
{
    using Criteria.Report;
    using Interfaces;
    using Models;
    using Models.Report;
    using Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ReportService : BaseService, IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly BaseService baseService;

        public ReportService(IReportRepository reportRepository, BaseService baseService)
        {
            this.reportRepository = reportRepository;
            this.baseService = baseService;
        }

        public IList<TimeRecord> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            IList<TimeRecord> timeRecords = null;
            if (criteria != null && criteria.IsValid())
            {
                timeRecords = reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords ?? new List<TimeRecord>();
        }

        public bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria)
        {
            return baseService.GetDataEntity<bool>(criteria);
        }

        public IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria)
        {
            IList<LateInEarlyOutModel> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = reportRepository.GetLateInEarlyOut(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutModel>();
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria)
        {
            IList<LateInEarlyOutDetail> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = reportRepository.GetLateInEarlyOutDetail(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDetail>();
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria)
        {
            IList<RequestsMonitoring> requestsMonitorings = null;
            if (criteria != null && criteria.IsValid())
            {
                requestsMonitorings = reportRepository.GetRequestMonitoring(criteria);
            }

            return requestsMonitorings ?? new List<RequestsMonitoring>();
        }

        public IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria)
        {
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetTotalWorkingHours(criteria);
            }

            return totalWorkingHours ?? new List<TotalWorkingHours>();
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria)
        {
            IList<EmployeeAttendanceModel> attendances = null;
            if (criteria != null && criteria.IsValid())
            {
                attendances = reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return attendances ?? new List<EmployeeAttendanceModel>();
        }

        public IList<LateInEarlyOutDiscipline> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDiscipline>();
        }

        public IList<WorkingHoursStatistic> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria)
        {
            IList<WorkingHoursStatistic> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = reportRepository.GetWorkingHoursStatistic(criteria);
            }

            return totalWorkingHours ?? new List<WorkingHoursStatistic>();
        }

        public IList<HeadCount> GetTeamHeadCountInYear(GetHeadCountByYearCriteria criteria)
        {
            IList<HeadCount> headCount = GetData<HeadCount>(criteria);
            return headCount;
        }

        public IList<YearlyNexRequest> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria)
        {
            List<YearlyNexRequest> listYearlyNexRequest = new List<YearlyNexRequest>();
            if (criteria != null && criteria.IsValid())
            {
                listYearlyNexRequest = reportRepository.GetYearlyNexRequest(criteria).ToList();
            }

            return listYearlyNexRequest;
        }
    }
}