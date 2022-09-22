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

        public async Task<Employee> GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = await employeeRepository.GetById(criteria);
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = await employeeRepository.GetByDomainName(criteria);
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByNickName(GetEmployeeByNickNameCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = await employeeRepository.GetByNickName(criteria);
            }

            return employee;
        }

        public async Task<IList<Employee>> Query(GetEmployeeCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = await employeeRepository.Query(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public async Task<IEnumerable<Employee>> GetMembersByManager(GetMemberByManagerCriteria criteria)
        {
            IEnumerable<Employee> employees = await baseService.GetDataAsync<Employee>(criteria);
            return employees;
        }

        public async Task<DefaultSchedule> GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            DefaultSchedule defaultSchedule = await baseService.GetDataEntityAsync<DefaultSchedule>(criteria);
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

        public async Task<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            HRLeaderDashboardModel dashboard = await baseService.GetDataEntityAsync<HRLeaderDashboardModel>(criteria);
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

        public async Task<IEnumerable<Employee>> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria)
        {
            IEnumerable<Employee> employees = await baseService.GetDataAsync<Employee>(criteria);
            return employees;
        }

        public async Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria)
        {
            IEnumerable<MissingCheckInOutModel> missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutModel>(criteria);
            return missingCheckInOuts;
        }

        public async Task<IEnumerable<EmployeeLateInEarlyOut>> GetEmployeeLateInEarlyOut(GetEmployeeAttendanceLateInEarlyOutCriteria criteria)
        {
            IEnumerable<EmployeeLateInEarlyOut> employees = await baseService.GetDataAsync<EmployeeLateInEarlyOut>(criteria);
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

        public async Task<IList<Workload>> GetWorkloadDefaultByTeam(GetEmployeeWorkloadCriteria criteria)
        {
            return await employeeRepository.GetWorkloadByTeam(criteria);
        }

        public async Task<IEnumerable<MissingCheckInOutEmployee>> GetMissingCheckInOutEmployeeAsync()
        {
            var missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutEmployee>(new GetMissingCheckInCheckOutCriteria()).ConfigureAwait(false);

            return missingCheckInOuts;
        }

        public async Task<IEnumerable<LuminetGroup>> GetLuminetGroups()
        {
            return await baseService.GetDataAsync<LuminetGroup>(new GetLuminetGroupCriteria());
        }

        public async Task<IEnumerable<LuminetShift>> GetLuminetShifts()
        {
            return await baseService.GetDataAsync<LuminetShift>(new GetLuminetShiftCriteria());
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