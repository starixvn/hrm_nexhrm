// --------------------------------------------------------------------------
// <copyright file="ReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

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
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Report repository.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        /// <summary>
        /// Get employee attendance report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of employee attendance model.</returns>
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

        /// <summary>
        /// Get request monitoring.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of request monitoring.</returns>
        public IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria)
        {
            IList<RequestsMonitoring> entityList = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entityList = db.Query<RequestsMonitoring, Employee, Team, RequestsMonitoring>(
                    (rq, e, g) =>
                    {
                        rq.Requester = e;
                        e.Team = g;
                        return rq;
                    },
                    splitOn: "EmpId, GroupId",
                    parameterValues: criteria.GetSpParams()).ToList();
            }

            return entityList;
        }

        /// <summary>
        /// Get working hour list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of working hours.</returns>
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

        /// <summary>
        /// Get annual leave balance report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of leave summary.</returns>
        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = new List<LeaveSummary>();

                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LeaveSummary
                            {
                                Employee = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid"),
                                    JoinedDate = reader.ToDateTime("JoinedDate"),
                                    ConfirmedDate = reader.ToDateTime("ConfirmedDate")
                                },
                                AnnualLeaveLastYear = reader.ToFloat("AnnualLeavelastYear"),
                                AnnualLeaveCurrentYear = reader.ToFloat("AnnualLeaveCurrentYear"),
                                Jan = reader.ToFloat("Jan"),
                                Feb = reader.ToFloat("Feb"),
                                Mar = reader.ToFloat("Mar"),
                                Apr = reader.ToFloat("Apr"),
                                May = reader.ToFloat("May"),
                                Jun = reader.ToFloat("Jun"),
                                Jul = reader.ToFloat("Jul"),
                                Aug = reader.ToFloat("Aug"),
                                Sep = reader.ToFloat("Sep"),
                                Oct = reader.ToFloat("Oct"),
                                Nov = reader.ToFloat("Nov"),
                                Dec = reader.ToFloat("Dec"),
                                LeaveBalance = reader.ToFloat("LeaveBalance"),
                                Remark = reader.ToString("Remark")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        public IList<TimeRecordModel> GetTimeRecords(ICriteria criteria)
        {
            IList<TimeRecordModel> timeRecords = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                timeRecords = db.Query<TimeRecordModel, Employee, Team, TimeRecordModel>(
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
            }

            return timeRecords;
        }

        public IList<LateInEarlyOutSummary> GetLateInEarlyOutSummary(ICriteria criteria)
        {
            IList<LateInEarlyOutSummary> lateinEarlyOuts = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                lateinEarlyOuts = db.Query<LateInEarlyOutSummary, Employee, Team, LateInEarlyOutSummary>(
                    (la, e, g) =>
                    {
                        la.Employee = e;
                        e.Team = g;
                        return la;
                    },
                    splitOn: "EmpId, GroupId",
                    parameterValues: criteria.GetSpParams()).ToList();
            }

            return lateinEarlyOuts;
        }

        public IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria)
        {
            IList<LateInEarlyOutDetail> lateInEarlyOuts = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                lateInEarlyOuts = db.Query<LateInEarlyOutDetail, Employee, Team, LateInEarlyOutDetail>(
                    (la, e, g) =>
                    {
                        la.Employee = e;
                        e.Team = g;
                        return la;
                    },
                    splitOn: "EmpId, GroupId",
                    parameterValues: criteria.GetSpParams()).ToList();
            }

            return lateInEarlyOuts;
        }

        public IList<LeaveDeductionDetail> GetLeaveDeductionDetail(ICriteria criteria)
        {
            IList<LeaveDeductionDetail> leaveDeductions = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                leaveDeductions = db.Query<LeaveDeductionDetail, Employee, Team, Employee, LeaveDeductionDetail>(
                    (le, e, g, modi) =>
                    {
                        le.Employee = e;
                        e.Team = g;
                        le.ModifiedBy = modi;
                        return le;
                    },
                    splitOn: "EmpId, GroupId, ModifiedById",
                    parameterValues: criteria.GetSpParams()).ToList();
            }

            return leaveDeductions;
        }
    }
}