namespace Fanex.NexHR.Repositories
{
    using Criteria.Overtime;
    using Data;
    using Interfaces;
    using Models;
    using System.Collections.Generic;

    public class OvertimeRepository : IOvertimeRepository
    {
        public IEnumerable<Overtime> GetSummaryReport(GetSummaryCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Employee, Overtime>(
                    (o, e1, e2) =>
                    {
                        o.CreatedBy = e1;
                        o.Employee = e2;
                        return o;
                    },
                    "CreatedId, EmployeeId",
                    criteria.GetSpParams());

                return result;
            }
        }
       
        public Overtime Get(GetByIdCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var model = db.QueryEntity<Overtime, Employee, Employee, Employee, Overtime>(
                    (o, e1, e2, e3) =>
                    {
                        o.CreatedBy = e1;
                        o.ModifiedBy = e2;
                        o.Employee = e3;
                        return o;
                    },
                    "CreatedId, ModifiedId, EmployeeId",
                    criteria.GetSpParams());

                return model;
            }
        }       
    }
}