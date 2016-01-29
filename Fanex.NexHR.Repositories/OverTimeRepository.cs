namespace Fanex.NexHR.Repositories
{
    using Data;
    using Data.Repository;
    using Core.Enums;
    using Criteria.OverTime;
    using Models;
    using Interfaces;
    using ViewModels;
    using System.Collections.Generic;

    public class OverTimeRepository : IOverTimeRepository
    {
        public bool Insert(ICriteria createCriteria)
        {
            using (var db = new ObjectDb(createCriteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(createCriteria.GetSpParams()) != 0;
            }
        }

        public IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model)
        {
            var criteria = new GetSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status,
                EmployeeId = model.EmployeeId
            };

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

        public bool UpdateStatus(int id, RequestStatuse status, int userId)
        {
            var criteria = new UpdateStatusCriteria
            {
                Id = id,
                Status = (int)status,
                UserId = userId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        public OverTime Get(int id)
        {
            var criteria = new GetByIdCriteria { Id = id };

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

        public bool Update(OverTimeViewModel model)
        {
            var criteria = new UpdateOverTimeCriteria
            {
                Id = model.Id,
                Description = model.Description,
                Date = model.Date,
                Hour = model.Hour,
                EmployeeId = model.EmployeeId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }
        public bool ApproveRequest(int id, int userId)
        {
            var criteria = new ApproveOverTimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Approve,
                UserId = userId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        public bool DisapproveRequest(int id, int userId)
        {
            var criteria = new DisapproveOverTimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Pending,
                UserId = userId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }
    }
}