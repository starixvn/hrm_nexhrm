namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.AnnualLeave;

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

        public IList<LeaveConversionDetail> GetLeaveConversionDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
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
                    parameterSet: criteria.GetSpParams()).ToList();

                return leaveDeductions;
            }
        }
    }
}