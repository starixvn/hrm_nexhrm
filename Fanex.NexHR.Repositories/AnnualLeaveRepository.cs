namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.AnnualLeave;

    public class AnnualLeaveRepository : IAnnualLeaveRepository
    {
        public async Task<IList<LeaveSummary>> GetAnnualLeaveBalance(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var leaveSummmary = await db.QueryAsync<LeaveSummary, Employee, Team, LeaveSummary>(
                     (leave, employee, team) =>
                     {
                         employee.Team = team;
                         leave.Employee = employee;
                         return leave;
                     },
                     "EmpId, GroupId",
                     criteria);

                return leaveSummmary.ToList();
            }
        }

        public async Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var leaveTransactions = await db.QueryAsync<AnnualLeaveTransaction, Employee, AnnualLeaveTransaction>(
                     (leaveTrans, employee) =>
                     {
                         leaveTrans.Employee = employee;
                         return leaveTrans;
                     },
                     "EmpId",
                     criteria);

                return leaveTransactions.ToList();
            }
        }

        public AnnualLeaveTransaction GetAnnualLeaveTransactionById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var leaveTransaction = db.QueryEntity<AnnualLeaveTransaction, Employee, AnnualLeaveTransaction>(
                     (leaveTrans, employee) =>
                     {
                         leaveTrans.Employee = employee;
                         return leaveTrans;
                     },
                     "EmpId",
                     criteria);

                return leaveTransaction;
            }
        }

        public IList<LeaveConversionDetail> GetLeaveConversionDetail(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var leaveDeductions = db.Query<LeaveConversionDetail, Employee, Team, Employee, LeaveConversionDetail>(
                    (le, e, g, modi) =>
                    {
                        le.Employee = e;
                        e.Team = g;
                        le.ModifiedBy = modi;
                        return le;
                    },
                    splitOn: "EmpId, GroupId, ModifiedById",
                    parameterSet: criteria).ToList();

                return leaveDeductions;
            }
        }
    }
}