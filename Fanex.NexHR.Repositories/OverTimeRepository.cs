namespace Fanex.NexHR.Repositories
{
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Overtime;
    using System.Collections.Generic;
    using System.Linq;

    public class OvertimeRepository : IOvertimeRepository
    {
        private const string stringSplitOnEmployeeId = "EmployeeId";

        public IList<Overtime> GetOvertime(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
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

        public Overtime GetOvertimeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
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

        public IList<OvertimeSummary> GetOvertimeSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var overtimeSummmary = db.Query<OvertimeSummary, Employee, Team, OvertimeSummary>(
                     (overtime, employee, team) =>
                     {
                         employee.Team = team;
                         overtime.Employee = employee;
                         return overtime;
                     },
                     "EmpId, GroupId",
                     criteria.GetSpParams()).ToList();

                return overtimeSummmary;
            }
        }

        public IList<OvertimeConversion> GetConversions(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var conversions = db.Query<OvertimeConversion, Employee, Team, Employee, OvertimeConversion>(
                    (otc, e1, t, e2) =>
                    {
                        e1.Team = t;
                        otc.Employee = e1;
                        otc.ModifiedBy = e2;
                        return otc;
                    },
                    "EmpId, GroupId, ModifiedById",
                    criteria.GetSpParams()).ToList();

                return conversions;
            }
        }

        public IList<ReportOvertimeHRDetail> GetReportOvertimeHRDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var report = db.Query<ReportOvertimeHRDetail, Employee, ReportOvertimeHRDetail>(
                    (rp, e1) =>
                    {
                        rp.Employee = e1;
                        return rp;
                    },
                    stringSplitOnEmployeeId,
                    criteria.GetSpParams()).ToList();

                return report;
            }
        }
    }
}