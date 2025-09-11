namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using HRM.Criteria.Employee;
    using Models;
    using Models.Report;
    using Models.Workload;
    using ViewModels;

    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Task<UserSession> GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria);

        Task<IEnumerable<EmployeeInfo>> GetEmployeeByUsername(GetEmployeeByUsernameCriteria criteria);

        Task<IEnumerable<EmployeeInfo>> GetByEmployeeModifiedDate(GetEmployeeByModifiedDateCriteria criteria);

        Task<Employee> GetEmployeeByNickName(GetEmployeeByNickNameCriteria criteria);

        Task<IList<Employee>> Query(GetEmployeeCriteria criteria);

        Task<MySpaceViewModel> GetMySpace(GetMySpaceCriteria criteria);

        Task<IList<Employee>> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        Task<IEnumerable<Employee>> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);

        Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria);

        Task<IEnumerable<EmployeeLateInEarlyOut>> GetEmployeeLateInEarlyOut(GetEmployeeAttendanceLateInEarlyOutCriteria criteria);

        Task<IList<Workload>> GetWorkloadDefaultByTeam(GetEmployeeWorkloadCriteria criteria);

        Task<IEnumerable<MissingCheckInOutEmployee>> GetMissingCheckInOutEmployeeAsync();

        Task<IEnumerable<LuminetGroup>> GetLuminetGroups();

        Task<IEnumerable<LuminetShift>> GetLuminetShifts();

        Task<IEnumerable<EmployeesMissedLogTime>> GetEmployeesMissedLogTime();

        IList<string> UserRoles(UserSession currentUser);

        Task<CommandResult> CheckExistAsync(CheckExistEmployeeCriteria criteria);
    }
}