namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    public class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IList<EmployeeAttendanceModel> attendances = multi.Read<EmployeeAttendanceModel>().ToList();

                    IEnumerable<RequestEntity> requests = multi.Read<RequestEntity, RequestTypeModel, Shift, Shift, Employee, LeaveType, RequestEntity>(
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

                    PropertyInfo totalitem = criteria.GetType().GetProperties().Where(
                        prop => Attribute.IsDefined(prop, typeof(OutputAttribute)) && prop.Name.ToLower().Equals("totalitem")).FirstOrDefault();

                    if (totalitem != null)
                    {
                        var hashParam = db.GetOuputParams();
                        totalitem.SetValue(criteria, (int)hashParam["totalitem"], null);
                    }

                    foreach (EmployeeAttendanceModel attendance in attendances)
                    {
                        attendance.Requests = requests.Where(t => (t.Requester.Id == attendance.EmpId
                            && (attendance.AttendanceDate.Value.Date - t.FromDate.Date).Days >= 0
                            && (t.ToDate.Date - attendance.AttendanceDate.Value.Date).Days >= 0)).ToList();
                        attendance.IsMissingCheckin = (attendance.IsMissingCheckin == 0 || (attendance.Requests.Where(t => t.RequestType.Id == 1).Count() > 0)) ? 0 : 1;
                        attendance.IsMissingCheckOut = (attendance.IsMissingCheckOut == 0 || (attendance.Requests.Where(t => t.RequestType.Id == 2).Count() > 0)) ? 0 : 1;
                    }

                    return attendances;
                }
            }
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
                    parameterValues: criteria.GetSpParams()).ToList();

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
                    parameterValues: criteria.GetSpParams()).ToList();

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
                    parameterValues: criteria.GetSpParams()).ToList();

                PropertyInfo totalitem = criteria.GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(OutputAttribute)) && prop.Name.ToLower().Equals("totalitem")).FirstOrDefault();

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
                    parameterValues: criteria.GetSpParams()).ToList();

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
                    parameterValues: criteria.GetSpParams()).ToList();

                return lateInEarlyOuts;
            }
        }

        public IList<LateInEarlyOutDiscipline> GetLateInEarlyOutSummary(ICriteria criteria)
        {
            IList<LateInEarlyOutDiscipline> lateinEarlyOuts = null;

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
                       "Id, GroupId").ToList();

                    IList<LateInEarlyOutDisciplineDetail> lateInEarlyOutDetails = multi.Read<LateInEarlyOutDisciplineDetail>().ToList();

                    GroupLateInEarlyOutSummaryDetailByEmployeeId(lateinEarlyOuts, lateInEarlyOutDetails);
                }
            }

            return lateinEarlyOuts;
        }

        private void GroupLateInEarlyOutSummaryDetailByEmployeeId(
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