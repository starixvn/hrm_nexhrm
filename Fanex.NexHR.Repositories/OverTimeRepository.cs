namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Overtime;

    public class OvertimeRepository : IOvertimeRepository
    {
        private const string stringSplitOnEmployeeId = "EmployeeId";

        public async Task<IList<Overtime>> GetOvertime(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    IList<Overtime> overtimes = multi.Read<Overtime, Employee, Overtime>(
                        (ot, cre) =>
                        {
                            ot.CreatedBy = cre;
                            return ot;
                        },
                        "CreatedId").ToList();

                    IEnumerable<OvertimeRecord> overtimeRecords =
                        multi.Read<OvertimeRecord, Employee, OvertimeRecord>(
                       (ot, emp) =>
                       {
                           ot.Employee = emp;
                           return ot;
                       },
                        stringSplitOnEmployeeId);

                    foreach (Overtime overtime in overtimes)
                    {
                        overtime.OvertimeRecords = overtimeRecords.Where(t => t.OvertimeId == overtime.Id).ToList();
                    }

                    return overtimes;
                }
            }
        }

        public async Task<Overtime> GetOvertimeById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    Overtime overtimeRequest = multi.Read<Overtime, Employee, Overtime>(
                        (ot, cre) =>
                        {
                            ot.CreatedBy = cre;
                            return ot;
                        },
                        "CreatedId").FirstOrDefault();

                    IEnumerable<OvertimeRecord> overtimeRecords =
                        multi.Read<OvertimeRecord, Employee, OvertimeRecord>(
                       (ot, emp) =>
                       {
                           ot.Employee = emp;
                           return ot;
                       },
                        stringSplitOnEmployeeId);

                    if (overtimeRequest != null)
                    {
                        overtimeRequest.OvertimeRecords = overtimeRecords.ToList();
                    }

                    return overtimeRequest;
                }
            }
        }

        public async Task<IList<OvertimeSummary>> GetOvertimeSummary(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var overtimeSummmary = await db.QueryAsync<OvertimeSummary, Employee, Team, OvertimeSummary>(
                     (overtime, employee, team) =>
                     {
                         employee.Team = team;
                         overtime.Employee = employee;
                         return overtime;
                     },
                     "EmpId, GroupId",
                     criteria);

                return overtimeSummmary.ToList();
            }
        }

        public async Task<IList<OvertimeConversion>> GetConversions(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var conversions = await db.QueryAsync<OvertimeConversion, Employee, Team, Employee, OvertimeConversion>(
                    (otc, e1, t, e2) =>
                    {
                        e1.Team = t;
                        otc.Employee = e1;
                        otc.ModifiedBy = e2;
                        return otc;
                    },
                    "EmpId, GroupId, ModifiedById",
                    criteria);

                return conversions.ToList();
            }
        }
    }
}