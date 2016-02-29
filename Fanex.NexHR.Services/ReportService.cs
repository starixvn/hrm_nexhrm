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

        public IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            IList<TimeRecordModel> timeRecords = null;
            if (criteria != null && criteria.IsValid())
            {
                timeRecords = reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords ?? new List<TimeRecordModel>();
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

        public IList<LateInEarlyOutByTeamModel> GetLateInEarlyOutByTeam(IList<LateInEarlyOutModel> lateInEarlyOuts, IList<Team> teams)
        {
            IList<LateInEarlyOutByTeamModel> lateInEarlyOutByTeam = new List<LateInEarlyOutByTeamModel>();
            teams = teams.OrderBy(t => t.Rank).ToList();

            if (lateInEarlyOuts != null)
            {
                foreach (Team team in teams)
                {
                    LateInEarlyOutByTeamModel entity = new LateInEarlyOutByTeamModel
                    {
                        Team = team,
                        LateInEarlyOutInTeams = new List<LateInEarlyOutModel>()
                    };

                    foreach (LateInEarlyOutModel lateInEarlyOut in lateInEarlyOuts)
                    {
                        if (lateInEarlyOut.Employee.Team.Name == team.Name)
                        {
                            entity.LateInEarlyOutInTeams.Add(lateInEarlyOut);
                        }
                    }

                    lateInEarlyOutByTeam.Add(entity);
                }
            }

            return lateInEarlyOutByTeam;
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

        public IList<LateInEarlyOutSummaryModel> GetLateInEarlyOutSummary(GetLateInEarlyOutSummaryCriteria criteria)
        {
            IList<LateInEarlyOutSummaryModel> lateInEarlyOuts = null;
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts ?? new List<LateInEarlyOutSummaryModel>();
        }

        public IList<LateInEarlyOutSummaryByTeamModel> GetLateInEarlyOutSummaryByTeam(
            IList<LateInEarlyOutSummaryModel> lateInEarlyOuts,
            IList<Team> teams)
        {
            IList<LateInEarlyOutSummaryByTeamModel> lateInEarlyOutByTeam = new List<LateInEarlyOutSummaryByTeamModel>();
            teams = teams.OrderBy(t => t.Rank).ToList();

            if (lateInEarlyOuts != null)
            {
                foreach (Team team in teams)
                {
                    LateInEarlyOutSummaryByTeamModel entity = new LateInEarlyOutSummaryByTeamModel
                    {
                        Team = team,
                        LateInEarlyOutSummaryInTeams = new List<LateInEarlyOutSummaryModel>()
                    };

                    foreach (LateInEarlyOutSummaryModel lateInEarlyOut in lateInEarlyOuts)
                    {
                        if (lateInEarlyOut.Employee.Team.Name == team.Name)
                        {
                            entity.LateInEarlyOutSummaryInTeams.Add(lateInEarlyOut);
                        }
                    }

                    lateInEarlyOutByTeam.Add(entity);
                }
            }

            return lateInEarlyOutByTeam;
        }
    }
}