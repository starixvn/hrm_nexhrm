namespace Fanex.NexHR.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Workload;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Workload;
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
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetById(criteria);
            }

            return employee;
        }

        public Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetByDomainName(criteria);
            }

            return employee;
        }

        public IList<Employee> Query(GetEmployeeCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
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
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria != null && criteria.IsValid())
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

        public IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                if (criteria.Enable != null)
                {
                    criteria.Enable = criteria.Enable != true;
                }

                employees = employeeRepository.GetEmployeeToeicScore(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel employees = null;
            if (criteria != null && criteria.IsValid())
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

        public IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria)
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
            var criteria = new GetDashBoardCompanySpaceCriteria()
            {
                EmpId = employeeId
            };

            return employeeRepository.GetDashBoardCompanySpace(criteria);
        }

        public IList<Workload> GetWorkloadByTeam(GetEmployeeWorkloadCriteria criteria)
        {
            return employeeRepository.GetWorkloadByTeam(criteria);
        }
    }
}