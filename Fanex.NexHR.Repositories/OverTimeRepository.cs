namespace Fanex.NexHR.Repositories
{
    using Criteria.OverTime;
    using Data;
    using Interfaces;
    using Models;
    using System.Collections.Generic;

    public class OverTimeRepository : IOverTimeRepository
    {
        public IEnumerable<OverTime> GetSummaryReport(GetSummaryCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<OverTime, Employee, Employee, OverTime>(
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
       
        public OverTime Get(GetByIdCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var model = db.QueryEntity<OverTime, Employee, Employee, Employee, OverTime>(
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