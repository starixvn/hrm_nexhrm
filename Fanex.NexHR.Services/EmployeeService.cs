// --------------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
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

    /// <summary>
    /// Employee service.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository = null)
        {
            this._employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public IList<Employee> GetLeaders(GetLeadersOfEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> leaders = new List<Employee>();
            if (criteria.IsValid())
            {
                leaders = this._employeeRepository.GetLeaders(criteria);
            }

            return leaders;
        }

        public IList<Employee> GetManagements(GetManagementsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> managements = new List<Employee>();
            if (criteria.IsValid())
            {
                managements = this._employeeRepository.GetManagements(criteria);
            }

            return managements;
        }

        public IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = new List<Employee>();

            if (criteria.IsValid())
            {
                employees = GetData<Employee>(criteria).ToList();
            }

            return employees;
        }

        /// <summary>
        /// Get Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Employee Entity.</returns>
        public Employee GetEmployeeById(GetEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = new Employee();
            if (criteria.IsValid())
            {
                if ((criteria.Id != null && criteria.Id != -1) || criteria.DomainUserName != null)
                {
                    employee = this._employeeRepository.GetById(criteria);
                }
            }

            return employee;
        }

        public IList<Employee> Query(GetAllEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.Query(criteria);
            }

            return lstEmployee;
        }

        public IList<Employee> GetEmployeeForRelatedTeam(GetEmployeeForRelatedTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.GetEmployeeForRelatedTeam(criteria);
            }

            return lstEmployee;
        }

        public int Insert(InsertEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int inserted = -1;
            if (criteria.IsValid())
            {
                inserted = this._employeeRepository.Insert(criteria);
            }

            return inserted;
        }

        public bool Delete(DeleteEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool deleted = false;
            if (criteria.IsValid())
            {
                deleted = this._employeeRepository.Delete(criteria);
            }

            return deleted;
        }

        public bool Update(UpdateEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.Update(criteria);
            }

            return updated;
        }

        public EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            EmployeeShift model = null;
            if (criteria.IsValid())
            {
                model = this._employeeRepository.GetEmployeeShift(criteria);
            }

            return model;
        }

        public bool UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.UpdateEmployeeShift(criteria);
            }

            return updated;
        }

        public MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria.IsValid())
            {
                space = this._employeeRepository.GetMySpace(criteria);
            }

            return space;
        }

        public HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            HRLeaderDashboardModel dashboard = new HRLeaderDashboardModel();
            if (criteria.IsValid())
            {
                dashboard = this._employeeRepository.GetHRLeaderDashboard(criteria);
            }

            return dashboard;
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleModel> workingSchedule = new List<WorkingScheduleModel>();
            if (criteria.IsValid())
            {
                workingSchedule = this._employeeRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedule;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendar = new List<WorkingCalendarsViewModel>();
            if (criteria.IsValid())
            {
                workingCalendar = this._employeeRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendar;
        }

        public bool UpdateShift(UpdateShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.UpdateShift(criteria);
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
            return this._employeeRepository.GetAnnualLeaveTransactions(criteria);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveBalanceModel model = null;

            if (criteria.IsValid())
            {
                model = this._employeeRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return model;
        }

        public bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.DeleteAnnualLeaveTransaction(criteria);
        }

        public bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.UpdateAnnualLeaveTransaction(criteria);
        }

        public bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.InsertAnnualLeaveTransaction(criteria);
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
            return this._employeeRepository.GetUserChangedShift(criteria);
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria)
        {
            return this._employeeRepository.GetHalfDayOffs(criteria);
        }

        /// <summary>
        /// Get range date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The range date.</returns>
        public int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            return this._employeeRepository.GetRangeDate(criteria);
        }

        /// <summary>
        /// Get disable request date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of date.</returns>
        public IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            return this._employeeRepository.GetDisableRequestDate(criteria);
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

            return this._employeeRepository.GetEmployeeToeicScore(criteria);
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
                lstEmployee = this._employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
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
                lstEmployee = this._employeeRepository.GetUpdatingScoreRelatedPeople(criteria);
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
    }
}