namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Core.Attributes;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Request;

    public class RequestRepository : BaseRepository, IRequestRepository
    {
        public IList<RequestEntity> GetRequest(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            entityList.Add(GetRequestInfo(reader));
                        }
                    }
                }

                PropertyInfo totalitem = criteria.GetType()
                    .GetProperties()
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(OutputAttribute)) &&
                                            prop.Name.ToLower(CultureInfo.InvariantCulture).Equals("totalitem"));

                if (totalitem != null)
                {
                    var hashParam = db.GetOutputParams();
                    totalitem.SetValue(criteria, (int)hashParam["totalitem"], null);
                }
            }

            return entityList;
        }

        public RequestEntity GetRequestEntity(ICriteria criteria)
        {
            RequestEntity request = new RequestEntity();
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            request = GetRequestInfo(reader);
                        }
                    }
                }
            }

            return request;
        }

        private RequestEntity GetRequestInfo(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                Status = new RequestStatus { Id = reader.GetByte(reader.GetOrdinal("Status")) },
                RequestType = new RequestTypeModel { Id = reader.GetInt16(reader.GetOrdinal("RequestType")) },
                TotalDaysRequestInMonth = !reader.IsDBNull(reader.GetOrdinal("TotalDaysRequestInMonth")) ? reader.GetDecimal(reader.GetOrdinal("TotalDaysRequestInMonth")) : 0,
                IsAllowUpdateStatus = !reader.IsDBNull(reader.GetOrdinal("IsAllowUpdateStatus")) &&
                                        reader.GetByte(reader.GetOrdinal("IsAllowUpdateStatus")) == 1,
                RequestedDate = reader.GetDateTime(reader.GetOrdinal("RequestedDate")),
                Requester = new Employee
                {
                    Id = reader.GetInt32(reader.GetOrdinal("empID")),
                    NickName = reader.GetString(reader.GetOrdinal("Requester")),
                    FullName = reader.GetString(reader.GetOrdinal("FullNameRequester")),
                    Email = reader.GetString(reader.GetOrdinal("EmailRequester")),
                    AnnualLeaveBalance = reader.GetDecimal(reader.GetOrdinal("AnnualLeaveBalance")),
                    Team = new Team { Name = reader.GetString(reader.GetOrdinal("Team")), Code = reader.GetString(reader.GetOrdinal("TeamCode")) }
                },
                ModifiedBy = new Employee
                {
                    NickName = GetStringFromReader(reader, "ProcessedBy"),
                    FullName = GetStringFromReader(reader, "FullNameProcessedBy"),
                    Email = GetStringFromReader(reader, "EmailProcessedBy")
                },
                ModifiedDate = !reader.IsDBNull(reader.GetOrdinal("ProcessedDate")) ? reader.GetDateTime(reader.GetOrdinal("ProcessedDate")) : DateTime.Now,
                FromShift = new Shift { Name = GetStringFromReader(reader, "FromShift") },
                ToShift = new Shift { Name = GetStringFromReader(reader, "ToShift") },
                TotalMins = !reader.IsDBNull(reader.GetOrdinal("TotalMins")) ? reader.GetInt32(reader.GetOrdinal("TotalMins")) : 0,
                FromDate = reader.GetDateTime(reader.GetOrdinal("FromDate")),
                ToDate = reader.GetDateTime(reader.GetOrdinal("ToDate")),
                TotalDays = reader.GetDecimal(reader.GetOrdinal("TotalDays")),
                LeaveType = new LeaveType
                {
                    Id = reader.GetInt32(reader.GetOrdinal("LeaveTypeId")),
                    Name = GetStringFromReader(reader, "LeaveTypeName")
                },
                Remark = GetStringFromReader(reader, "Remark"),
                IsCrossDate = !reader.IsDBNull(reader.GetOrdinal("IsCrossDate")) && reader.GetBoolean(reader.GetOrdinal("IsCrossDate"))
            };

            return entity;
        }

        private static string GetStringFromReader(IDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) ?
            reader.GetString(reader.GetOrdinal(name)) : string.Empty;
    }
}