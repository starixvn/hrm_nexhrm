namespace Fanex.NexHR.Repositories
{
    using Core.Attributes;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Report;
    using Models.Request;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IList<EmployeeAttendanceModel> attendances = multi.Read<EmployeeAttendanceModel>().ToList();

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

                    PropertyInfo totalitem = GetTotalItemPropertyInfo(criteria);

                    if (totalitem != null)
                    {
                        var hashParam = db.GetOuputParams();
                        totalitem.SetValue(criteria, (int)hashParam["totalitem"], null);
                    }

                    foreach (EmployeeAttendanceModel attendance in attendances)
                    {
                        attendance.Requests = GetAttendanceRequests(requests, attendance);
                        attendance.IsMissingCheckin = attendance.IsMissingCheckin == 0 ||
                                                      attendance.Requests.Any(t => t.RequestType.Id == 1)
                            ? 0
                            : 1;

                        attendance.IsMissingCheckOut = attendance.IsMissingCheckOut == 0 ||
                                                       attendance.Requests.Any(t => t.RequestType.Id == 2)
                            ? 0
                            : 1;
                    }

                    return attendances;
                }
            }
        }

        private static PropertyInfo GetTotalItemPropertyInfo(ICriteria criteria)
        {
            return criteria.GetType()
                .GetProperties()
                .FirstOrDefault(prop =>
                    Attribute.IsDefined(prop, typeof(OutputAttribute))
                    && prop.Name.ToLower(CultureInfo.InvariantCulture).Equals("totalitem"));
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
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<RequestsMonitoring, Employee, Team, RequestsMonitoring>(
                        (rq, e, g) =>
                        {
                            rq.Requester = e;
                            e.Team = g;
                            return rq;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return entityList;
            }
        }

        public IList<TotalWorkingHours> GetTotalWorkingHours(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<TotalWorkingHours, Employee, Team, TotalWorkingHours>(
                        (wkh, e, g) =>
                        {
                            wkh.Employee = e;
                            e.Team = g;
                            return wkh;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return entityList;
            }
        }

        public IList<TimeRecord> GetTimeRecords(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var timeRecords = db.Query<TimeRecord, Employee, Team, TimeRecord>(
                        (t, e, g) =>
                        {
                            t.Employee = e;
                            e.Team = g;
                            return t;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                PropertyInfo totalitem = criteria.GetType()
                    .GetProperties()
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(OutputAttribute))
                                            && prop.Name.ToLower(CultureInfo.InvariantCulture).Equals("totalitem"));

                if (totalitem != null)
                {
                    var hashParam = db.GetOuputParams();
                    totalitem.SetValue(criteria, (int)hashParam["totalitem"], null);
                }

                return timeRecords;
            }
        }

        public IList<LateInEarlyOutModel> GetLateInEarlyOut(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var lateinEarlyOuts = db.Query<LateInEarlyOutModel, Employee, Team, LateInEarlyOutModel>(
                        (la, e, g) =>
                        {
                            la.Employee = e;
                            e.Team = g;
                            return la;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return lateinEarlyOuts;
            }
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var lateInEarlyOuts = db.Query<LateInEarlyOutDetail, Employee, Team, LateInEarlyOutDetail>(
                        (la, e, g) =>
                        {
                            la.Employee = e;
                            e.Team = g;
                            return la;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return lateInEarlyOuts;
            }
        }

        public IList<LateInEarlyOutDiscipline> GetLateInEarlyOutSummary(ICriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateinEarlyOuts;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
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
    }
}