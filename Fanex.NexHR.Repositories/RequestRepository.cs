// --------------------------------------------------------------------------
// <copyright file="RequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Request Repository.
    /// </summary>
    public class RequestRepository : BaseRepository, IRequestRepository
    {
        public IList<RequestEntity> GetRequest(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            entityList.Add(GetRequestInfo(reader));
                        }
                    }
                }
            }

            return entityList;
        }

        public RequestEntity GetRequestEntity(ICriteria criteria)
        {
            RequestEntity request = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            request = GetRequestInfo(reader);
                            return request;
                        }
                    }
                }
            }

            return request;
        }

        public decimal GetNumberRequestInMonth(ICriteria criteria)
        {
            decimal numberRequest = 0;

            //// implement latter

            return numberRequest;
        }

        private RequestEntity GetRequestInfo(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("Id"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                IsAllowUpdateStatus = reader.ToInt("IsAllowUpdateStatus") == 1,
                RequestedDate = reader.ToDateTime("RequestedDate"),
                Requester = new Employee
                {
                    Id = reader.To<int>("empID"),
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Email = reader.To<string>("EmailRequester"),
                    AnnualLeaveBalance = reader.ToDecimal("AnnualLeaveBalance"),
                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                },
                ModifiedBy = new Employee
                {
                    NickName = reader.To<string>("ProcessedBy"),
                    FullName = reader.To<string>("FullNameProcessedBy"),
                    Email = reader.To<string>("EmailProcessedBy")
                },
                ModifiedDate = reader.ToDateTime("ProcessedDate"),
                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                TotalMins = reader.ToInt("TotalMins"),
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDays = reader.ToDecimal("TotalDays"),
                LeaveType = new LeaveType
                {
                    Id = reader.ToInt("LeaveTypeId"),
                    Name = reader.ToString("LeaveTypeName")
                },
                Remark = reader.ToString("Remark")
            };

            return entity;
        }
    }
}
