namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Repositories.Interfaces;

    public class AnnualLeaveRepository : IAnnualLeaveRepository
    {
        public IList<LeaveSummary> GetAnnualLeaveBalance(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var leaveSummmary = db.Query<LeaveSummary, Employee, Team, LeaveSummary>(
                     (leave, employee, team) =>
                     {
                         employee.Team = team;
                         leave.Employee = employee;
                         return leave;
                     },
                     "EmpId, GroupId",
                     criteria.GetSpParams()).ToList();

                return leaveSummmary;
            }
        }

        public IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var leaveTransactions = db.Query<AnnualLeaveTransaction, Employee, AnnualLeaveTransaction>(
                     (leaveTrans, employee) =>
                     {
                         leaveTrans.Employee = employee;
                         return leaveTrans;
                     },
                     "EmpId",
                     criteria.GetSpParams()).ToList();

                return leaveTransactions;
            }
        }

        public AnnualLeaveTransaction GetAnnualLeaveTransactionById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var leaveTransaction = db.QueryEntity<AnnualLeaveTransaction, Employee, AnnualLeaveTransaction>(
                     (leaveTrans, employee) =>
                     {
                         leaveTrans.Employee = employee;
                         return leaveTrans;
                     },
                     "EmpId",
                     criteria.GetSpParams());

                return leaveTransaction;
            }
        }
    }
}