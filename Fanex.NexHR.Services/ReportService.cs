namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.Report;
    using Interfaces;
    using Models;
    using Models.Report;
    using Repositories;
    using Repositories.Interfaces;

    public class ReportService : BaseService, IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService()
            : this(null)
        {
        }

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public IList<TimeRecord> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            IList<TimeRecord> timeRecords = null;
            if (criteria != null && criteria.IsValid())
            {
                timeRecords = _reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords ?? new List<TimeRecord>();
        }

        public IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria)
        {
            IList<LateInEarlyOutModel> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOut(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutModel>();
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria)
        {
            IList<LateInEarlyOutDetail> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOutDetail(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDetail>();
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria)
        {
            IList<RequestsMonitoring> requestsMonitorings = null;
            if (criteria != null && criteria.IsValid())
            {
                requestsMonitorings = _reportRepository.GetRequestMonitoring(criteria);
            }

            return requestsMonitorings ?? new List<RequestsMonitoring>();
        }

        public IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria)
        {
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetTotalWorkingHours(criteria);
            }

            return totalWorkingHours ?? new List<TotalWorkingHours>();
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria)
        {
            IList<EmployeeAttendanceModel> attendances = null;
            if (criteria != null && criteria.IsValid())
            {
                attendances = _reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return attendances ?? new List<EmployeeAttendanceModel>();
        }

        public IList<LateInEarlyOutDiscipline> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutDiscipline>();
        }

        public IList<WorkingHoursStatistic> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria)
        {
            IList<WorkingHoursStatistic> totalWorkingHours = null;
            if (criteria != null && criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetWorkingHoursStatistic(criteria);
            }

            return totalWorkingHours ?? new List<WorkingHoursStatistic>();
        }
    }
}