namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Fanex.Data.Repository;
    using Interfaces;
    using Models;

    public class OvertimeRepository : IOvertimeRepository
    {
        public IList<Overtime> GetOvertime(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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
                        "EmployeeId");

                    foreach (Overtime overtime in overtimes)
                    {
                        overtime.OvertimeRecords = overtimeRecords.Where(t => t.Id == overtime.Id).ToList();
                    }

                    return overtimes;
                }
            }
        }

        public Overtime GetOvertimeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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
                        "EmployeeId");

                    overtimeRequest.OvertimeRecords = overtimeRecords.ToList();

                    return overtimeRequest;
                }
            }
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employeeOvertimes = db.Query<EmployeeOvertime, Employee, Employee, EmployeeOvertime>(
                    (o, e, cr) =>
                    {
                        o.Employee = e;
                        o.CreatedBy = cr;
                        return o;
                    },
                    "EmployeeId, CreatedId",
                    criteria.GetSpParams()).ToList();

                return employeeOvertimes;
            }
        }
    }
}