namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class ReportService : BaseService, IReportService
    {
        private readonly IReportRepository reportRepository;

        public ReportService()
            : this(null)
        {
        }

        public ReportService(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository ?? new ReportRepository();
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
    }
}