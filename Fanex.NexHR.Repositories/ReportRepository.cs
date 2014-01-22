// --------------------------------------------------------------------------
// <copyright file="ReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report repository.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            int? groupId = null,
            int? excludeUserDelete = 1)
        {
            using (var db = new ObjectDb("EmployeeAttendance_GetByType"))
            {
                return db.Query<EmployeeAttendanceModel>(
                    new { fromTime, toTime, type, empId, groupId, excludeUserDelete }).ToList();
            }
        }

        public IList<RequestsMonitoring> GetRequestMonitoring(DateTime fromDate, DateTime toDate, int maxRequest, int role, int employeeID)
        {
            using (var db = new ObjectDb("RequestMonitoring"))
            {
                var entityList = new List<RequestsMonitoring>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, maxRequest, role, employeeID }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestsMonitoring
                            {
                                Requester = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.ToString("Team"), Id = reader.ToInt("TeamId") },
                                    Id = reader.ToInt("EmpID")
                                },
                                TotalCheckInOut = reader.ToInt("TotalCheckInOutRequest"),
                                TotalLateInEarlyOut = reader.ToInt("TotalLateInEarlyOutRequest"),
                                TotalChangeShift = reader.ToInt("TotalChangeShiftRequest"),
                                TotalLeave = reader.ToInt("TotalLeaveRequest")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        public IList<TotalWorkingHours> GetWorkingHoursList(DateTime fromDate, DateTime toDate, int? empId, int? groupId)
        {
            using (var db = new ObjectDb("Report_WorkingHoursSummary"))
            {
                var entityList = new List<TotalWorkingHours>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, empId, groupId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new TotalWorkingHours
                            {
                                Employee = new Employee
                                {
                                    Code = reader.ToString("Code"),
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid")
                                },
                                WorkingDay = reader.ToInt("WorkingDays"),
                                ActualWorkingDay = reader.ToFloat("ActualWorkingDays"),
                                LeaveTaken = reader.ToFloat("LeaveTaken"),
                                UnpaidLeave = reader.ToFloat("UnpaidLeave"),
                                LateInMins = reader.ToInt("LateIn"),
                                EarlyOutMins = reader.ToInt("EarlyOut"),
                                TotalHours = reader.ToFloat("TotalWorkHours"),
                                AverageHours = reader.ToFloat("AverageHours")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(int year, int? empId, int? groupId)
        {
            using (var db = new ObjectDb("Report_LeaveSummary"))
            {
                var entityList = new List<LeaveSummary>();

                using (IDataReader reader = db.ExecuteReader(new { empId, groupId, year }))
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
    }
}
