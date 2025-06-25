namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
    using Fanex.NexHR.Models.SyncData.SyncNexGoalModel;
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

        Task<IEnumerable<SyncEmployeeModel>> GetEmployeesByLastModifiedDate(GetEmployeesByLastModifiedDateCriteria criteria);

        Task<IEnumerable<SyncEmployeeAttendanceModel>> GetEmployeeAttendances(
            GetEmployeeAttendancesCriteria criteria);

        Task<IEnumerable<EmployeeCalendarModel>> GetEmployeeCalendarsByLastModifiedDate(GetEmployeeCalendarsByLastModifiedDateCriteria criteria);

        Task<IEnumerable<EmployeeOverTimeModel>> GetEmployeeOvertimesByLastModifiedDate(GetEmployeeOvertimeByLastModifiedDateCriteria criteria);

        Task<IEnumerable<EmployeeTOEIC>> GetEmployeeTOEICsByLastModifiedDate(GetEmployeeTOEICByLastModifiedDateCriteria criteria);
    }
}