// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
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

        public CommandResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IList<LateInEarlyOutSummary> GetLateInEarlyOutSummary(GetLateInEarlyOutSummaryReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LateInEarlyOutSummary> lateInEarlyOuts = null;
            if (criteria.IsValid())
            {
                lateInEarlyOuts = _reportRepository.GetLateInEarlyOutSummary(criteria);
            }

            return lateInEarlyOuts;
        }

        public IList<LateInEarlyOutSummaryByTeam> GetLateInEarlyOutSummaryByTeam(IList<LateInEarlyOutSummary> lateInEarlyOuts, IList<Team> teams)
        {
            IList<LateInEarlyOutSummaryByTeam> lateInEarlyOutSummaryByTeam = new List<LateInEarlyOutSummaryByTeam>();
            teams = teams.OrderBy(t => t.Rank).ToList();

            foreach (Team team in teams)
            {
                LateInEarlyOutSummaryByTeam entity = new LateInEarlyOutSummaryByTeam
                {
                    Team = team,
                    LateInEarlyOutSummaryInTeam = new List<LateInEarlyOutSummary>()
                };

                foreach (LateInEarlyOutSummary lateInEarlyOut in lateInEarlyOuts)
                {
                    if (lateInEarlyOut.Employee.Team.Name == team.Name)
                    {
                        entity.LateInEarlyOutSummaryInTeam.Add(lateInEarlyOut);
                    }
                }

                lateInEarlyOutSummaryByTeam.Add(entity);
            }

            return lateInEarlyOutSummaryByTeam;
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
    }
}
