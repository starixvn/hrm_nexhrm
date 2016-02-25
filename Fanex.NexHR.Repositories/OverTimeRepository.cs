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
        public IEnumerable<Overtime> ReportDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Employee, Overtime>(
                    (o, e1, e2) =>
                    {
                        o.CreatedBy = e1;
                        ////o.Employee = e2;
                        return o;
                    },
                    "CreatedId, EmployeeId",
                    criteria.GetSpParams());

                return result;
            }
        }

        public IEnumerable<Overtime> ReportSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Overtime>(
                    (o, e1) =>
                    {
                        o.CreatedBy = e1;
                        return o;
                    },
                    "CreatedId",
                    criteria.GetSpParams());

                return result;
            }
        }

        public IEnumerable<Overtime> GetMyReport(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Overtime>(
                    (o, e1) =>
                    {
                        o.CreatedBy = e1;
                        return o;
                    },
                    "CreatedId",
                    criteria.GetSpParams());

                return result;
            }
        }

        public Overtime Get(ICriteria criteria)
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
                        "EmployeeId").FirstOrDefault();

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
    }
}