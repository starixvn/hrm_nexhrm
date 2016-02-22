namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository = null)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public Employee GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employee = new Employee();
            if (criteria.IsValid())
            {
                employee = employeeRepository.GetById(criteria);
            }

            return employee;
        }

        public Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employee = new Employee();
            if (criteria.IsValid())
            {
                employee = employeeRepository.GetByDomainName(criteria);
            }

            return employee;
        }

        public IList<Employee> Query(GetEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = new List<Employee>();
            if (criteria.IsValid())
            {
                employees = employeeRepository.Query(criteria);
            }

            return employees;
        }

        public CommandResult Insert(InsertEmployeeCriteria criteria)
        {
            return Execute(criteria, hasGetOutputParams: true);
        }

        public CommandResult Delete(DeleteEmployeeCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult Update(UpdateEmployeeCriteria criteria)
        {
            return Execute(criteria);
        }

        public IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = null;

            if (criteria.IsValid())
            {
                employees = GetData<Employee>(criteria).ToList();
            }

            return employees ?? new List<Employee>();
        }

        public DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            DefaultSchedule defaultSchedule = null;
            if (criteria.IsValid())
            {
                defaultSchedule = GetDataEntity<DefaultSchedule>(criteria);
            }

            return defaultSchedule;
        }

        public CommandResult UpdateEmployeeShift(UpdateScheduleDefaultCriteria criteria)
        {
            return Execute(criteria);
        }

        public MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria.IsValid())
            {
                space = employeeRepository.GetMySpace(criteria);
            }

            return space;
        }

        public HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            HRLeaderDashboardModel dashboard = new HRLeaderDashboardModel();
            if (criteria.IsValid())
            {
                dashboard = GetDataEntity<HRLeaderDashboardModel>(criteria);
            }

            return dashboard;
        }

        public ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<ShowOpenAndCloseAnnualBalance>(criteria.GetSpParams());

                return entity;
            }
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<AnnualLeaveBalanceModel> annualLeaves = null;

            if (criteria.IsValid())
            {
                annualLeaves = GetData<AnnualLeaveBalanceModel>(criteria).ToList();
            }

            return annualLeaves ?? new List<AnnualLeaveBalanceModel>();
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            AnnualLeaveBalanceModel annualLeave = null;
            if (criteria.IsValid())
            {
                annualLeave = GetDataEntity<AnnualLeaveBalanceModel>(criteria);
            }

            return annualLeave;
        }

        public CommandResult DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult AddLeaveDeduction(InsertLeaveDeductionCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult EditLeaveDeduction(UpdateLeaveDeductionCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult DeleteLeaveDeduction(DeleteLeaveDeductionCriteria criteria)
        {
            return Execute(criteria);
        }

        public int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            return employeeRepository.GetRangeDate(criteria);
        }

        public IList<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<DateTime> dates = null;
            if (criteria.IsValid())
            {
                dates = GetData<DateTime>(criteria).ToList();
            }

            return dates ?? new List<DateTime>();
        }

        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaves, IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeams = new List<LeaveSummaryByTeam>();

            teams = teams.OrderBy(t => t.Rank).ToList();

            foreach (Team team in teams)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaves)
                {
                    if (leavesumary.Employee.Team.Name == team.Name)
                    {
                        entity.LeaveSummaryInTeam.Add(leavesumary);
                    }
                }

                leaveSummaryByTeams.Add(entity);
            }

            return leaveSummaryByTeams;
        }

        public IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            if (criteria.Enable != null)
            {
                criteria.Enable = criteria.Enable == true ? false : true;
            }

            return employeeRepository.GetEmployeeToeicScore(criteria);
        }

        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel lstEmployee = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                lstEmployee = employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            }

            return lstEmployee;
        }

        public IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = null;
            if (criteria.IsValid())
            {
                employees = GetData<Employee>(criteria).ToList();
            }

            return employees ?? new List<Employee>();
        }

        public CommandResult AdjustAnnualLeaves(AdjustAnnualLeavesCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult OpenAnnualLeaves(OpenAnnualBalanceForNewYearCriteria criteria)
        {
            return Execute(criteria);
        }

        public CommandResult ResetAnnualLeaves(ResetAnnualLeavesCriteria criteria)
        {
            return Execute(criteria);
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpID(GetMissingCheckInOutEmployeesCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<MissingCheckInOutModel> missingCheckInOuts = null;
            if (criteria.IsValid())
            {
                missingCheckInOuts = GetData<MissingCheckInOutModel>(criteria).ToList();
            }

            return missingCheckInOuts ?? new List<MissingCheckInOutModel>();
        }
    }
}