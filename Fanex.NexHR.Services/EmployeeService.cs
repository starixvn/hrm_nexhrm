namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
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
            Employee employee = null;
            if (criteria.IsValid())
            {
                employee = employeeRepository.GetById(criteria);
            }

            return employee;
        }

        public Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = null;
            if (criteria.IsValid())
            {
                employee = employeeRepository.GetByDomainName(criteria);
            }

            return employee;
        }

        public IList<Employee> Query(GetEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = null;
            if (criteria.IsValid())
            {
                employees = employeeRepository.Query(criteria);
            }

            return employees ?? new List<Employee>();
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

        public IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            if (criteria.Enable != null)
            {
                criteria.Enable = criteria.Enable != true;
            }

            return employeeRepository.GetEmployeeToeicScore(criteria);
        }

        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel employees = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                employees = employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            }

            return employees;
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