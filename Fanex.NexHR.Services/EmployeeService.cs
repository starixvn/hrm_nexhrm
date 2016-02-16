namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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

        public IList<Employee> Query(GetAllEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = employeeRepository.Query(criteria);
            }

            return lstEmployee;
        }

        public CommandResult Insert(InsertEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult Delete(DeleteEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult Update(UpdateEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
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

        public CommandResult UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
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

        public IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleModel> workingSchedule = new List<WorkingScheduleModel>();
            if (criteria.IsValid())
            {
                workingSchedule = employeeRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedule;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendar = new List<WorkingCalendarsViewModel>();
            if (criteria.IsValid())
            {
                workingCalendar = employeeRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendar;
        }

        public bool UpdateShift(UpdateShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = employeeRepository.UpdateShift(criteria);
            }

            return updated;
        }

        public bool InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Insert(criteria);
                return result.Code == 0;
            }

            return false;
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

        public CommandResult AddLeaveDeduction(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Insert(criteria);
                return result;
            }

            return new CommandResult()
            {
                Code = 1,
                Message = Labels.InvalidTotalDays
            };
        }

        public CommandResult EditLeaveDeduction(UpdateLeaveDeductionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Update(criteria);
                return result;
            }

            return new CommandResult()
            {
                Code = 1,
                Message = Labels.InvalidTotalDays
            };
        }

        public CommandResult DeleteLeaveDeduction(DeleteLeaveDeductionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = Repository.Delete(criteria);
                return result;
            }

            return new CommandResult()
            {
                Code = 1,
                Message = Labels.InvalidTotalDays
            };
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            return employeeRepository.GetUserChangedShift(criteria);
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria)
        {
            return employeeRepository.GetHalfDayOffs(criteria);
        }

        /// <summary>
        /// Get range date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The range date.</returns>
        public int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            return employeeRepository.GetRangeDate(criteria);
        }

        /// <summary>
        /// Get disable request date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of date.</returns>
        public IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            return employeeRepository.GetDisableRequestDate(criteria);
        }

        #region Get leave summary by team

        /// <summary>
        /// This function use for sorting leave summaries. Any leave summary of employee will be grouped by team. (For easier displaying in view).
        /// </summary>
        /// <param name="leaveSummaryList">Leave summary List.</param>
        /// <param name="listTeam">Team List.</param>
        /// <returns>List Annual Leave.</returns>
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

        #endregion Get leave summary by team

        public IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            if (criteria.Enable != null)
            {
                criteria.Enable = criteria.Enable == true ? false : true;
            }

            return employeeRepository.GetEmployeeToeicScore(criteria);
        }

        /// <summary>
        /// Function to get employees has birthday and first working.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of employee has birthday and list employee has first working day.</returns>
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

        /// <summary>
        /// Function to get all related people in updating TOEIC score.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of related people.</returns>
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