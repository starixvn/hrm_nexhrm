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
            DefaultSchedule employeeShift = null;
            if (criteria.IsValid())
            {
                employeeShift = GetDataEntity<DefaultSchedule>(criteria);
            }

            return employeeShift;
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
            return employeeRepository.GetAnnualLeaveTransactions(criteria);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveBalanceModel model = null;

            if (criteria.IsValid())
            {
                model = employeeRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return model;
        }

        public bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria)
        {
            return employeeRepository.DeleteAnnualLeaveTransaction(criteria);
        }

        public bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria)
        {
            return employeeRepository.UpdateAnnualLeaveTransaction(criteria);
        }

        public bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria)
        {
            return employeeRepository.InsertAnnualLeaveTransaction(criteria);
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

        public IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            return employeeRepository.GetDisableRequestDate(criteria);
        }

        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> listTeam)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();

            listTeam = listTeam.OrderBy(t => t.Rank).ToList();

            foreach (Team team in listTeam)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList)
                {
                    if (leavesumary.Employee.Team.Name == team.Name)
                    {
                        entity.LeaveSummaryInTeam.Add(leavesumary);
                    }
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
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
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = employeeRepository.GetUpdatingScoreRelatedPeople(criteria);
            }

            return lstEmployee;
        }

        public bool AdjustAnnualLeaves(AdjustAnnualLeavesCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Update(criteria);
                return result.Code == 0;
            }

            return false;
        }

        public bool OpenAnnualLeaves(OpenAnnualBalanceForNewYearCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Insert(criteria);
                return result.Code == 0;
            }

            return false;
        }

        public bool ResetAnnualLeaves(ResetAnnualLeavesCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Update(criteria);
                return result.Code == 0;
            }

            return false;
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpID(GetMissingCheckInOutEmployeesCriteria criteria)
        {
            if (criteria.IsValid())
            {
                return employeeRepository.GetMissingCheckInOutEmployees(criteria);
            }

            return new List<MissingCheckInOutModel>();
        }
    }
}