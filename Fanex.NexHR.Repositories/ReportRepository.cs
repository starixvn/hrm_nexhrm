namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Attributes;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Report;
    using Models.Request;

    public class ReportRepository : IReportRepository
    {
        private const string totalItem = "totalitem";
        private const string stringSplitOn = "EmpId, GroupId";
        private const int requestTypeId = 2;

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                IList<EmployeeAttendanceModel> attendances;

                using (var multi = db.QueryMultiple(criteria))
                {
                    attendances = multi.Read<EmployeeAttendanceModel>().ToList();

                    IEnumerable<RequestEntity> requests = multi
                        .Read<RequestEntity, RequestTypeModel, Shift, Shift, Employee, LeaveType, RequestEntity>(
                            (rq, ty, s1, s2, e, le) =>
                            {
                                rq.RequestType = ty;
                                rq.FromShift = s1;
                                rq.ToShift = s2;
                                rq.Requester = e;
                                rq.LeaveType = le;
                                return rq;
                            },
                            "RequestTypeId, FromShiftId, ToShiftId, RequesterId, LeaveTypeId");

                    foreach (EmployeeAttendanceModel attendance in attendances)
                    {
                        attendance.Requests = GetAttendanceRequests(requests, attendance);
                        attendance.IsMissingCheckin = attendance.IsMissingCheckin == 0 ||
                                                      attendance.Requests.Any(t => t.RequestType.Id == 1)
                            ? 0
                            : 1;

                        attendance.IsMissingCheckOut = attendance.IsMissingCheckOut == 0 ||
                                                       attendance.Requests.Any(t => t.RequestType.Id == requestTypeId)
                            ? 0
                            : 1;
                    }


                }

                PropertyInfo totalitem = GetTotalItemPropertyInfo(criteria);

                if (totalitem != null)
                {
                    var hashParam = db.GetOutputParams();
                    totalitem.SetValue(criteria, (int)hashParam[totalItem], null);
                }

                return attendances;
            }
        }

        private static PropertyInfo GetTotalItemPropertyInfo(ICriteria criteria)
        {
            return criteria.GetType()
                .GetProperties()
                .FirstOrDefault(prop =>
                    Attribute.IsDefined(prop, typeof(OutputAttribute))
                    && prop.Name.ToLower(CultureInfo.InvariantCulture).Equals(totalItem));
        }

        private static IList<RequestEntity> GetAttendanceRequests(
            IEnumerable<RequestEntity> requests,
            EmployeeAttendanceModel attendance)
        {
            return requests.Where(t =>
                    attendance.AttendanceDate != null
                    && (t.Requester.Id == attendance.EmpId
                        && (attendance.AttendanceDate.Value.Date - t.FromDate.Date).Days >= 0
                        && (t.ToDate.Date - attendance.AttendanceDate.Value.Date).Days >= 0))
                .ToList();
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = db.Query<RequestsMonitoring, Employee, Team, RequestsMonitoring>(
                        (rq, e, g) =>
                        {
                            rq.Requester = e;
                            e.Team = g;
                            return rq;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria)
                    .ToList();

                return entityList;
            }
        }

        public IList<TotalWorkingHours> GetTotalWorkingHours(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = db.Query<TotalWorkingHours, Employee, Team, TotalWorkingHours>(
                        (wkh, e, g) =>
                        {
                            wkh.Employee = e;
                            e.Team = g;
                            return wkh;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria)
                    .ToList();

                return entityList;
            }
        }

        public async Task<IList<TimeRecord>> GetTimeRecords(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var timeRecords = await db.QueryAsync<TimeRecord, Employee, Team, TimeRecord>(
                        (t, e, g) =>
                        {
                            t.Employee = e;
                            e.Team = g;
                            return t;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria);

                PropertyInfo totalitem = criteria.GetType()
                    .GetProperties()
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(OutputAttribute))
                                            && prop.Name.ToLower(CultureInfo.InvariantCulture).Equals(totalItem));

                if (totalitem != null)
                {
                    var hashParam = db.GetOutputParams();
                    totalitem.SetValue(criteria, (int)hashParam[totalItem], null);
                }

                return timeRecords.ToList();
            }
        }

        public IList<LateInEarlyOutModel> GetLateInEarlyOut(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var lateinEarlyOuts = db.Query<LateInEarlyOutModel, Employee, Team, LateInEarlyOutModel>(
                        (la, e, g) =>
                        {
                            la.Employee = e;
                            e.Team = g;
                            return la;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria)
                    .ToList();

                return lateinEarlyOuts;
            }
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var lateInEarlyOuts = db.Query<LateInEarlyOutDetail, Employee, Team, LateInEarlyOutDetail>(
                        (la, e, g) =>
                        {
                            la.Employee = e;
                            e.Team = g;
                            return la;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria)
                    .ToList();

                return lateInEarlyOuts;
            }
        }

        public IList<LateInEarlyOutDiscipline> GetLateInEarlyOutSummary(ICriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateinEarlyOuts;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    lateinEarlyOuts = multi.Read<LateInEarlyOutDiscipline, Employee, Team, LateInEarlyOutDiscipline>(
                            (la, e, g) =>
                            {
                                la.Employee = e;
                                e.Team = g;

                                return la;
                            },
                            "Id, GroupId")
                        .ToList();

                    IList<LateInEarlyOutDisciplineDetail> lateInEarlyOutDetails =
                        multi.Read<LateInEarlyOutDisciplineDetail>().ToList();

                    GroupLateInEarlyOutSummaryDetailByEmployeeId(lateinEarlyOuts, lateInEarlyOutDetails);
                }
            }

            return lateinEarlyOuts;
        }

        private static void GroupLateInEarlyOutSummaryDetailByEmployeeId(
            IList<LateInEarlyOutDiscipline> lateinEarlyOuts,
            IList<LateInEarlyOutDisciplineDetail> lateInEarlyOutDetails)
        {
            foreach (LateInEarlyOutDiscipline lateinEarlyOut in lateinEarlyOuts)
            {
                foreach (LateInEarlyOutDisciplineDetail lateInEarlyOutDetail in lateInEarlyOutDetails)
                {
                    if (lateInEarlyOutDetail.EmpId.Equals(lateinEarlyOut.Employee.Id))
                    {
                        lateinEarlyOut.LateInEarlyOutDisciplineDetails.Add(lateInEarlyOutDetail);
                    }
                }
            }
        }

        public IList<WorkingHoursStatistic> GetWorkingHoursStatistic(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = db.Query<WorkingHoursStatistic, Employee, Team, WorkingHoursStatistic>(
                        (wkh, e, g) =>
                        {
                            wkh.Employee = e;
                            e.Team = g;
                            return wkh;
                        },
                        splitOn: stringSplitOn,
                        parameterSet: criteria)
                    .ToList();

                return entityList;
            }
        }

        public IList<YearlyNexRequest> GetYearlyNexRequest(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var yearlyNexRequest = db.Query<YearlyNexRequest, Employee, Team, YearlyNexRequest>(
                     (yearly, employee, team) =>
                     {
                         employee.Team = team;
                         yearly.Employee = employee;
                         return yearly;
                     },
                     "EmpId, GroupId",
                     criteria).ToList();

                return yearlyNexRequest;
            }
        }
    }
}