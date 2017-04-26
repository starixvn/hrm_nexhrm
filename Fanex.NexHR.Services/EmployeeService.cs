namespace Fanex.NexHR.Services
{
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Interfaces;
    using Models;
    using Models.Workload;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;
    using ViewModels;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository = null)
        {
            _employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public Employee GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetById(criteria);
            }

            return employee;
        }

        public Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetByDomainName(criteria);
            }

            return employee;
        }

        public IList<Employee> Query(GetEmployeeCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = _employeeRepository.Query(criteria);
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

                employees = _employeeRepository.GetEmployeeToeicScore(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = _employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
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
            var criteria = new GetDashBoardPersonalSpaceCriteria
            {
                EmpId = employeeId
            };

            return _employeeRepository.GetDashBoardPersonalSpace(criteria);
        }

        private IList<DashBoardCompanySpace> GetDashBoardCompanySpace(int employeeId)
        {
            var criteria = new GetDashBoardCompanySpaceCriteria()
            {
                EmpId = employeeId
            };

            return _employeeRepository.GetDashBoardCompanySpace(criteria);
        }

        public IList<Workload> GetWorkloadByTeam(GetEmployeeWorkloadCriteria criteria)
        {
            return _employeeRepository.GetWorkloadByTeam(criteria);
        }
    }
}