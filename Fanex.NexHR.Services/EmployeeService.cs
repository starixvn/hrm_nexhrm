namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.EmployeeTransfer;
    using Interfaces;
    using Models;
    using Models.Report;
    using Models.Workload;
    using Repositories.Interfaces;
    using ViewModels;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBaseService baseService;

        public EmployeeService(IEmployeeRepository employeeRepository, IBaseService baseService)
        {
            this.employeeRepository = employeeRepository;
            this.baseService = baseService;
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

        public Employee GetEmployeeByNickName(GetEmployeeByNickNameCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetByNickName(criteria);
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
            IList<Employee> employees = baseService.GetData<Employee>(criteria);
            return employees;
        }

        public DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            DefaultSchedule defaultSchedule = baseService.GetDataEntity<DefaultSchedule>(criteria);
            return defaultSchedule;
        }

        public async Task<MySpaceViewModel> GetMySpace(GetMySpaceCriteria criteria)
        {
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria != null && criteria.IsValid())
            {
                space = await baseService.GetDataEntityAsync<MySpaceViewModel>(criteria);
                space.DashboardCompanySpace = await GetDashBoardCompanySpace(criteria.EmpId);
                space.DashboardEmployeeSpace = await GetDashBoardPersonalSpace(criteria.EmpId);
                if (space.DashboardCompanySpace.Any())
                {
                    space.CompanyWorkFromHome = getCompanyWorkFromHome(space.DashboardCompanySpace);
                }
            }

            return space;
        }

        public HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            HRLeaderDashboardModel dashboard = baseService.GetDataEntity<HRLeaderDashboardModel>(criteria);
            return dashboard;
        }

        public async Task<IList<Employee>> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = await employeeRepository.GetEmployeeToeicScore(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public async Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = await employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria).ConfigureAwait(false);
            }

            return employees;
        }

        public IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria)
        {
            IList<Employee> employees = baseService.GetData<Employee>(criteria);
            return employees;
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria)
        {
            IList<MissingCheckInOutModel> missingCheckInOuts = baseService.GetData<MissingCheckInOutModel>(criteria);
            return missingCheckInOuts;
        }

        public IList<EmployeeLateInEarlyOut> GetEmployeeLateInEarlyOut(GetEmployeeAttendanceLateInEarlyOutCriteria criteria)
        {
            IList<EmployeeLateInEarlyOut> employees = baseService.GetData<EmployeeLateInEarlyOut>(criteria);
            return employees;
        }

        private async Task<IList<DashBoardPersonalSpace>> GetDashBoardPersonalSpace(int employeeId)
        {
            var criteria = new GetDashBoardPersonalSpaceCriteria
            {
                EmpId = employeeId
            };

            return await employeeRepository.GetDashBoardPersonalSpace(criteria);
        }

        private async Task<IList<DashBoardCompanySpace>> GetDashBoardCompanySpace(int employeeId)
        {
            var criteria = new GetDashBoardCompanySpaceCriteria
            {
                EmpId = employeeId
            };

            return await employeeRepository.GetDashBoardCompanySpace(criteria);
        }

        public async Task<IList<Workload>> GetWorkloadByTeam(GetEmployeeWorkloadCriteria criteria)
        {
            return await employeeRepository.GetWorkloadByTeam(criteria);
        }

        public async Task<IEnumerable<MissingCheckInOutEmployee>> GetMissingCheckInOutEmployeeAsync()
        {
            var missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutEmployee>(new GetMissingCheckInCheckOutCriteria()).ConfigureAwait(false);

            return missingCheckInOuts;
        }

        public IList<LuminetGroup> GetLuminetGroups()
        {
            return baseService.GetData<LuminetGroup>(new GetLuminetGroupCriteria());
        }

        public IList<LuminetShift> GetLuminetShifts()
        {
            return baseService.GetData<LuminetShift>(new GetLuminetShiftCriteria());
        }

        private static string getCompanyWorkFromHome(IList<DashBoardCompanySpace> dashBoard)
        {
            const float numberEqual = 0.5f;
            const int numberLength = 2;
            var employees = new StringBuilder();
            IList<DashBoardCompanySpace> workFromHomes = dashBoard
                .Where(t => t.IsWorkFromHome && t.IsCurrent && t.TypeId == (int)DashBoardType.None)
                .OrderBy(t => t.Employee.NickName)
                .ToList();

            IList<DashBoardCompanySpace> leaveDay = dashBoard
                .Where(t => t.IsCurrent && t.TypeId == (int)DashBoardType.Leave)
                .OrderBy(t => t.Employee.NickName)
                .ToList();

            foreach (DashBoardCompanySpace workFromHome in workFromHomes)
            {
                var wfhAndLeaveRequest = leaveDay.Where(x => workFromHome.Employee.Id == x.Employee.Id).ToList();
                if (!wfhAndLeaveRequest.Any())
                {
                    employees.Append(workFromHome.Employee.NickName);
                    employees.Append(", ");
                }
            }
            var leaveHalfDay = leaveDay.Where(x => x.LeaveDay.Equals(numberEqual));
            var workFromHomesHalfDay = workFromHomes.Where(x => leaveHalfDay.Any(y => y.Employee.Id == x.Employee.Id)).ToList();

            foreach (DashBoardCompanySpace workFromHome in workFromHomesHalfDay)
            {
                employees.Append(workFromHome.Employee.NickName);
                employees.Append(" (½)");
                employees.Append(", ");
            }

            return employees.Length > numberLength ? employees.ToString().Substring(0, employees.Length - numberLength) : string.Empty;
        }

        public IList<string> UserRoles(Employee currentUser)
        {
            const int secondCase = 2;
            const int threeCase = 3;
            const int fourCase = 4;

            var roles = new List<string>
            {
                Role.Member.EnumToString()
            };

            if (currentUser.HasRolePM)
            {
                roles.Add(Role.PM.EnumToString());
            }

            if (currentUser.RoleList != null)
            {
                foreach (var item in currentUser.RoleList)
                {
                    switch (item)
                    {
                        default:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            break;

                        case secondCase:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            roles.Add(Role.Leader.EnumToString());
                            break;

                        case threeCase:
                            roles.Add(Role.HR.EnumToString());
                            break;

                        case fourCase:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            roles.Add(Role.Leader.EnumToString());
                            roles.Add(Role.HR.EnumToString());
                            roles.Add(Role.SA.EnumToString());
                            break;
                    }
                }
            }

            return roles.ToArray();
        }
    }
}