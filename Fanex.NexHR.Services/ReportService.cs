namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
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
        private readonly IReportRepository _reportRepository;

        public ReportService()
            : this(null)
        {
        }

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TimeRecordModel> timeRecords = null;
            if (criteria.IsValid())
            {
                timeRecords = _reportRepository.GetTimeRecords(criteria);
            }

            return timeRecords;
        }

        public IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LateInEarlyOutModel> lateInEarlyOuts = null;
            if (criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOut(criteria);
            }

            return lateInEarlyOuts;
        }

        public IList<LateInEarlyOutByTeamModel> GetLateInEarlyOutByTeam(IList<LateInEarlyOutModel> lateInEarlyOuts, IList<Team> teams)
        {
            IList<LateInEarlyOutByTeamModel> lateInEarlyOutByTeam = new List<LateInEarlyOutByTeamModel>();
            teams = teams.OrderBy(t => t.Rank).ToList();

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

            return lateInEarlyOutByTeam;
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LateInEarlyOutDetail> lateInEarlyOuts = null;
            if (criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOutDetail(criteria);
            }

            return lateInEarlyOuts;
        }

        public IList<LeaveDeductionDetail> GetLeaveDeductionDetail(GetLeaveDeductionDetailCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LeaveDeductionDetail> leaveDeductions = null;
            if (criteria.IsValid())
            {
                leaveDeductions = _reportRepository.GetLeaveDeductionDetail(criteria);
            }

            return leaveDeductions;
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestsMonitoring> monitorings = null;
            if (criteria.IsValid())
            {
                monitorings = _reportRepository.GetRequestMonitoring(criteria);
            }

            return monitorings;
        }

        public IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<TotalWorkingHours> totalWorkingHours = null;
            if (criteria.IsValid())
            {
                totalWorkingHours = _reportRepository.GetTotalWorkingHours(criteria);
            }

            return totalWorkingHours;
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EmployeeAttendanceModel> attendances = null;

            if (criteria.IsValid())
            {
                attendances = _reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return attendances;
        }

        public IList<LateInEarlyOutSummaryModel> GetLateInEarlyOutSummary(GetLateInEarlyOutSummaryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LateInEarlyOutSummaryModel> lateInEarlyOuts = null;

            if (criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts;
        }

        public IList<LateInEarlyOutSummaryByTeamModel> GetLateInEarlyOutSummaryByTeam(
            IList<LateInEarlyOutSummaryModel> lateInEarlyOuts,
            IList<Team> teams)
        {
            IList<LateInEarlyOutSummaryByTeamModel> lateInEarlyOutByTeam = new List<LateInEarlyOutSummaryByTeamModel>();
            teams = teams.OrderBy(t => t.Rank).ToList();

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

            return lateInEarlyOutByTeam;
        }
    }
}