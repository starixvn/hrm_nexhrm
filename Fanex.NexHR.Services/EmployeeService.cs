namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
            Employee employee = GetDataEntity<Employee>(criteria);
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

        public IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria)
        {
            IList<Employee> employees = GetData<Employee>(criteria);
            return employees;
        }

        public DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            DefaultSchedule defaultSchedule = GetDataEntity<DefaultSchedule>(criteria);
            return defaultSchedule;
        }

        public MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria.IsValid())
            {
                space = GetDataEntity<MySpaceViewModel>(criteria);
                space.DashboardCompanySpace = GetDashBoardCompanySpace(criteria.EmpId);
                space.DashboardEmployeeSpace = GetDashBoardPersonalSpace(criteria.EmpId);
            }

            return space;
        }

        public HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            HRLeaderDashboardModel dashboard = GetDataEntity<HRLeaderDashboardModel>(criteria);
            return dashboard;
        }

        public int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            return employeeRepository.GetRangeDate(criteria);
        }

        public IList<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            IList<DateTime> dates = GetData<DateTime>(criteria);
            return dates;
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
            IList<Employee> employees = GetData<Employee>(criteria);
            return employees;
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpID(GetMissingCheckInOutEmployeesCriteria criteria)
        {
            IList<MissingCheckInOutModel> missingCheckInOuts = GetData<MissingCheckInOutModel>(criteria);
            return missingCheckInOuts;
        }

        private IList<DashBoardPersonalSpace> GetDashBoardPersonalSpace(int employeeId)
        {
            var criteria = new GetDashBoardPersonalSpaceCriteria()
            {
                EmpId = employeeId
            };

            return employeeRepository.GetDashBoardPersonalSpace(criteria);
        }

        private IList<DashBoardCompanySpace> GetDashBoardCompanySpace(int employeeId)
        {
            var criteria = new GetDashBoardPersonalSpaceCriteria()
            {
                EmpId = employeeId
            };

            return employeeRepository.GetDashBoardCompanySpace(criteria);
        }
    }
}