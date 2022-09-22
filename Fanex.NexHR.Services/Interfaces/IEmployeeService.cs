namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Models;
    using Models.Report;
    using Models.Workload;
    using ViewModels;

    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Task<Employee> GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria);

        Task<Employee> GetEmployeeByNickName(GetEmployeeByNickNameCriteria criteria);

        Task<IList<Employee>> Query(GetEmployeeCriteria criteria);

        Task<IEnumerable<Employee>> GetMembersByManager(GetMemberByManagerCriteria criteria);

        Task<DefaultSchedule> GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        Task<MySpaceViewModel> GetMySpace(GetMySpaceCriteria criteria);

        Task<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        Task<IList<Employee>> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        Task<IEnumerable<Employee>> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);

        Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria);

        Task<IEnumerable<EmployeeLateInEarlyOut>> GetEmployeeLateInEarlyOut(GetEmployeeAttendanceLateInEarlyOutCriteria criteria);

        Task<IList<Workload>> GetWorkloadDefaultByTeam(GetEmployeeWorkloadCriteria criteria);

        Task<IEnumerable<MissingCheckInOutEmployee>> GetMissingCheckInOutEmployeeAsync();

        Task<IEnumerable<LuminetGroup>> GetLuminetGroups();

        Task<IEnumerable<LuminetShift>> GetLuminetShifts();

        IList<string> UserRoles(Employee currentUser);
    }
}