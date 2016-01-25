using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Criteria.OverTime;
using Fanex.NexHR.Models;
using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NexHR.Repositories
{
    public class OverTimeRepository : IOverTimeRepository
    {
        public bool Insert(ICriteria createCriteria)
        {

            using (var db = new ObjectDb(createCriteria.GetSettingKey()))
            {
                db.ExecuteScalar(createCriteria.GetSpParams());

                return true;
            }
        }

        public IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model)
        {
            var criteria = new GetSummaryCriteria()
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

        public bool ApproveRequest(int id, int approvedUserId)
        {
            var criteria = new ApproveRequestCriteria()
            {
                Id = id,
                Status = (int)OTRequestStatus.Approved,
                ApprovedUserId = approvedUserId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteScalar(criteria.GetSpParams());

                return true;
            }
        }

        public bool VerifyRequest(int id, int verifiedUserId)
        {
            var criteria = new VerifyRequestCriteria()
            {
                Id = id,
                Status = (int)OTRequestStatus.Verified,
                VerifiedUserId = verifiedUserId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteScalar(criteria.GetSpParams());

                return true;
            }
        }

        public OverTime Get(int id)
        {
            var criteria = new GetByIdCriteria() { Id = id };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var model = db.QueryEntity<OverTime, Employee, Employee, Employee, Employee, OverTime>(
                    (o, e1, e2, e3, e4) =>
                    {
                        o.CreatedBy = e1;
                        o.ApprovedBy = e2;
                        o.VerifiedBy = e3;
                        o.Employee = e4;
                        return o;
                    },
                    "CreatedId, ApprovedId, VerifiedId, EmployeeId",
                    criteria.GetSpParams());

                return model;
            }
        }

        public bool Update(OverTimeViewModel model)
        {
            var createCriteria = new UpdateOverTimeCriteria()
            {
                Id = model.Id,
                Description = model.Description,
                Date = model.Date,
                Hour = model.Hour,
                EmployeeId = model.EmployeeId
            };

            using (var db = new ObjectDb(createCriteria.GetSettingKey()))
            {
                db.ExecuteScalar(createCriteria.GetSpParams());

                return true;
            }
        }
    }
}