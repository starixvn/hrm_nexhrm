namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Criteria.Employee;
    using Criteria.Workload;
    using Models;
    using Models.Workload;
    using ViewModels;

    public interface IEmployeeService
    {
        Employee GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria);

        IList<Employee> Query(GetEmployeeCriteria criteria);

        IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria);

        DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpId(GetMissingCheckInOutEmployeesCriteria criteria);

        Task<IEnumerable<MissingCheckInOutModel>> GetMissingCheckInOutAsync(GetMissingCheckInOutEmployeesCriteria criteria);

        IList<Workload> GetWorkloadByTeam(GetEmployeeWorkloadCriteria criteria);
    }
}