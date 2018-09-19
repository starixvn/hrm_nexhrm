namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Models;
    using Models.Workload;
    using ViewModels;
    using Models.Report;

    public interface IEmployeeService
    {
        Employee GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria);

        Employee GetEmployeeByNickName(GetEmployeeByNickNameCriteria criteria);

        IList<Employee> Query(GetEmployeeCriteria criteria);

        IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria);

        DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria);

        IList<EmployeeLateInEarlyOut> GetEmployeeLateInEarlyOut(GetEmployeeAttendanceLateInEarlyOutCriteria criteria);

        IList<Workload> GetWorkloadByTeam(GetEmployeeWorkloadCriteria criteria);
    }
}