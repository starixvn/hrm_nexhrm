// --------------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.23">Created</change>
//   <change who="Anders.Le" date="2012.5.23">Modify Get function</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.Data.Dapper;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee repository.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Get employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Employee object.</returns>
        public Employee Get(GetEmployeeCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<Employee, dynamic, Employee>(
                    (e, t) =>
                    {
                        e.Team = new Team { Id = t.TeamID, Name = t.Team };
                        return e;
                    }, 
                    "TeamID",
                    criteria.GetSpParams());

                return entity;
            }
        }

        /// <summary>
        /// Query employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Employee.</returns>
        public IList<Employee> Query(GetAllEmployeeCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         return e;
                     },
                     "TeamID",
                     criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        /// <summary>
        /// Get employee for related team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetEmployeeForRelatedTeam(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee>(criteria.GetSpParams()).ToList();

                return entityList;
            }
        }

        /// <summary>
        /// Get employee by teams.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="teamCodes">The team code.</param>
        /// <param name="ignoreTeams">List of ignored team.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetEmployeeByTeams(int employeeId, string teamCodes, string ignoreTeams)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeByTeams"))
            {
                var entityList = db.Query<Employee>(new { employeeId = employeeId, teamCodes = teamCodes, ignoreTeams = ignoreTeams }).ToList();

                return entityList;
            }
        }

        /// <summary>
        /// Get leaders.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetLeaders(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Employee>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Insert employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public int Insert(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return (int)db.ExecuteScalar(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Delete employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Delete(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria) != 0;
            }
        }

        /// <summary>
        /// Update employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Update(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0 ? true : false;
            }
        }

        /// <summary>
        /// Get employee shift.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The employee shift.</returns>
        public EmployeeShift GetEmployeeShift(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<EmployeeShift>(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Update employee shift.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool UpdateEmployeeShift(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Get week working.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <param name="date">The date value.</param>
        /// <returns>List of week working.</returns>
        public IList<WeekWorking> GetWeekWorking(int id, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetWeekWorking"))
            {
                return db.Query<WeekWorking>(new { id, date }).ToList();
            }
        }

        /// <summary>
        /// Update week working.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <param name="date">The date value.</param>
        /// <returns>The result.</returns>
        public bool UpdateWeekWorking(EmployeeShift model, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_UpdateWeekWorking"))
            {
                db.AddInParameter("@id", DbType.Int32, model.EmpID);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@monShift", DbType.Int32, model.MonShift);
                db.AddInParameter("@tueShift", DbType.Int32, model.TueShift);
                db.AddInParameter("@wedShift", DbType.Int32, model.WedShift);
                db.AddInParameter("@thuShift", DbType.Int32, model.ThuShift);
                db.AddInParameter("@friShift", DbType.Int32, model.FriShift);
                db.AddInParameter("@satShift", DbType.Int32, model.SatShift);
                db.AddInParameter("@sunShift", DbType.Int32, model.SunShift);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="requestId">The request ID.</param>
        /// <param name="processedBy">Processed by.</param>
        /// <param name="processedDate">Processed date.</param>
        /// <param name="message">The error message.</param>
        /// <returns>The result.</returns>
        public int ProcessRequest(
            Commands command,
            long requestId,
            int processedBy,
            DateTime processedDate,
            out string message)
        {
            string key = string.Empty;
            string processedByKey = string.Empty;
            string processedDateKey = string.Empty;

            switch (command)
            {
                case Commands.Approve:
                    key = "WorkingTimeRequest_ApproveRequest";
                    processedByKey = "@ApprovedBy";
                    processedDateKey = "@ApprovedDate";
                    break;

                case Commands.Disapprove:
                    key = "WorkingTimeRequest_DisapproveRequest";
                    processedByKey = "@ModifiedBy";
                    processedDateKey = "@ModifiedDate";
                    break;

                case Commands.Reject:
                    key = "WorkingTimeRequest_RejectRequest";
                    processedByKey = "@ProcessedBy";
                    processedDateKey = "@ProcessedDate";
                    break;
            }

            using (var db = new ObjectDb(key))
            {
                db.AddInParameter("@RequestID", DbType.Int64, requestId);

                db.AddInParameter(processedByKey, DbType.Int32, processedBy);
                db.AddInParameter(processedDateKey, DbType.DateTime, processedDate);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;

                return (int)db.GetParameterValue("ErrCode");
            }
        }

        /// <summary>
        /// Get me space.
        /// </summary>
        /// <param name="empid">The employee ID.</param>
        /// <returns>MY space view model.</returns>
        public MySpaceViewModel GetMySpace(int empid)
        {
            using (var db = new ObjectDb("Report_EmployeeDashBoard"))
            {
                var date = DateTime.Now.ToShortDateString();
                return db.Query<MySpaceViewModel>(new { empid, date }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get HR leader dashboard.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>HR leader dashboard model.</returns>
        public HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null)
        {
            using (var db = new ObjectDb("Report_HRLeaderDashBoard"))
            {
                return db.Query<HrLeaderDashboardModel>(new { employeeId, date }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Working time request.
        /// </summary>
        /// <param name="empId">The employee ID.</param>
        /// <param name="requestType">The request type.</param>
        /// <param name="isPlanned">Is planned or not.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="fromShift">The from shift.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="toShift">The to shift.</param>
        /// <param name="leaveTypeId">The leave type ID.</param>
        /// <param name="totalDays">Total days.</param>
        /// <param name="lateInMins">The late in duration in minutes.</param>
        /// <param name="earlyOutMins">The early out duration in minutes.</param>
        /// <returns>The request ID.</returns>
        public long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            DateTime createdDate,
            int createdBy,
            out int errorCode,
            out string message,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Insert"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@IsPlanned", DbType.Boolean, isPlanned);
                db.AddInParameter("@Remark", DbType.String, remark);

                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@FromShift", DbType.Int32, fromShift);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ToShift", DbType.Int32, toShift);

                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveTypeId);
                db.AddInParameter("@TotalDays", DbType.Decimal, totalDays);
                db.AddInParameter("@LateInMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);

                db.AddInParameter("@CreatedDate", DbType.DateTime, createdDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, createdBy);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : (long)result;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return requestId;
            }
        }

        /// <summary>
        /// Get working time request list.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="requestType">The request type.</param>
        /// <param name="status">The status.</param>
        /// <returns>List of working time request.</returns>
        public IList<WorkingTimeRequest> GetWorkingTimeRequests(
            int employeeId,
            int? requestType = null,
            byte? status = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_GetByEmployee"))
            {
                return db.Query<WorkingTimeRequest>(new { id = employeeId, requestType, status }).ToList();
            }
        }

        /// <summary>
        /// Get working time request.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Working time request.</returns>
        public WorkingTimeRequest GetWorkingTimeRequest(long id)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Get"))
            {
                return db.Query<WorkingTimeRequest>(new { id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Delete working time request.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>The result.</returns>
        public bool DeleteWorkingTimeRequest(int id)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Delete"))
            {
                db.AddInParameter("@id", DbType.Int32, id);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Update request.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="isPlanned">Is planned or not.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="modifiedby">The modified by.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="fromShift">The from shift.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="toShift">The to shift.</param>
        /// <param name="leaveTypeId">The leave type ID.</param>
        /// <param name="totalDays">The total days.</param>
        /// <param name="lateInMins">The late in minutes.</param>
        /// <param name="earlyOutMins">The early out minutes.</param>
        /// <returns>The result.</returns>
        public bool UpdateRequest(
            long requestId,
            bool isPlanned,
            string remark,
            int modifiedby,
            DateTime fromDate,
            DateTime modifiedDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Update"))
            {
                db.AddInParameter("@Id", DbType.Int64, requestId);
                db.AddInParameter("@IsPlanned", DbType.Boolean, isPlanned);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@FromShift", DbType.Int32, fromShift);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ToShift", DbType.Int32, toShift);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveTypeId);
                db.AddInParameter("@TotalDays", DbType.Decimal, totalDays);
                db.AddInParameter("@LateInMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedby);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Get working schedule.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <param name="id">The employee ID.</param>
        /// <returns>List of working schedule model.</returns>
        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByMonth"))
            {
                var ds = new DataSet();

                db.AddInParameter("@reportDate", DbType.DateTime, date);
                db.AddInParameter("@employeeId", DbType.Int32, id);
                ds = db.ExecuteDataSet();
                var dttWorkingSchedule = ds != null ? ds.Tables[0] : null;

                var list = new List<WorkingScheduleModel>();
                if (dttWorkingSchedule.Rows.Count != 0)
                {
                    for (int i = 1; i <= dttWorkingSchedule.Columns.Count - 4; i++)
                    {
                        var workingDay = new WorkingScheduleModel
                        {
                            Title = dttWorkingSchedule.Rows[0][i + 3].ToString(),
                            Start = new DateTime(date.Year, date.Month, i).ToShortDateString()
                        };
                        list.Add(workingDay);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Get working calendar.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>List of working calendar view model.</returns>
        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(string date)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByMonth"))
            {
                var list = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                db.AddInParameter("@reportDate", DbType.DateTime, date);
                ds = db.ExecuteDataSet();
                var dttWorkingCalendars = ds != null ? ds.Tables[0] : null;
                var totalColumns = dttWorkingCalendars.Columns.Count;

                foreach (DataRow row in dttWorkingCalendars.Rows)
                {
                    var empWorkingCalendar = new WorkingCalendarsViewModel();

                    empWorkingCalendar.EmpId = int.Parse(row["Empid"].ToString());
                    empWorkingCalendar.FullName = row["fullname"].ToString();
                    empWorkingCalendar.NickName = row["nickname"].ToString();
                    empWorkingCalendar.Team = row["Team"].ToString();
                    empWorkingCalendar.Days = totalColumns - 4;

                    var days = new List<string>();

                    for (int i = 4; i < totalColumns; i++)
                    {
                        days.Add(row[i].ToString());
                    }

                    empWorkingCalendar.WorkingDays = days;

                    list.Add(empWorkingCalendar);
                }

                return list;
            }
        }

        /// <summary>
        /// Update shift.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <param name="date">The date value.</param>
        /// <param name="shift">The shift ID.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <returns>The result.</returns>
        public bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_Update"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, id);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@ShiftID", DbType.Int32, shift);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Get check in/out request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of check in/out.</returns>
        public IList<CheckInOut> GetCheckInOutRequest(ICriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new CheckInOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get late in early out request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of late in early out request.</returns>
        public IList<LateInEarlyOut> GetLateInEarlyOutRequest(ICriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LateInEarlyOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalMins = reader.ToInt("Time"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Remark = reader.ToString("Remark"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get change shift request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of change shift request.</returns>
        public IList<ChangingShift> GetChangeShiftRequest(ICriteria criteria)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new ChangingShift
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                Remark = reader.ToString("Remark"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get leave request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of leave request.</returns>
        public IList<Leave> GetLeaveRequest(ICriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new Leave
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get all request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of request entity.</returns>
        public IList<RequestEntity> GetAllRequest(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                Requester = new Employee
                                {
                                    Id = reader.To<int>("empID"),
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                TotalMins = reader.ToInt("Time"),
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark")
                            };
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        #region daily requests

        /// <summary>
        /// Get check in/out request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of check in/out.</returns>
        public IList<CheckInOut> GetCheckInOutRequestByDate(ICriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new CheckInOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get late in early out request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of late in early out request.</returns>
        public IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(ICriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();
            var settingKey = criteria.GetSettingKey();

            using (var db = new ObjectDb(settingKey))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LateInEarlyOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalMins = reader.ToInt("Time"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Remark = reader.ToString("Remark"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get change shift request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of change shift request.</returns>
        public IList<ChangingShift> GetChangeShiftRequestByDate(ICriteria criteria)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new ChangingShift
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                Remark = reader.ToString("Remark"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get leave request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of leave request.</returns>
        public IList<Leave> GetLeaveRequestByDate(ICriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new Leave
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get all request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of request entity.</returns>
        public IList<RequestEntity> GetAllRequestByDate(ICriteria criteria)
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
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                Requester = new Employee
                                {
                                    Id = reader.To<int>("empID"),
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                TotalMins = reader.ToInt("Time"),
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        #endregion

        /// <summary>
        /// Get check in/out request history.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of check in/out request.</returns>
        public IList<CheckInOut> GetCheckInOutRequestHistory(ICriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new CheckInOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get late in early out request history.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of late in early out request.</returns>
        public IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(ICriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LateInEarlyOut
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                Shift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                TotalMins = reader.ToInt("Time"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Remark = reader.ToString("Remark"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get change shift request history.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of change shift request.</returns>
        public IList<ChangingShift> GetChangeShiftRequestHistory(ICriteria criteria)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new ChangingShift
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Remark = reader.ToString("Remark"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestedDate = reader.ToDateTime("RequestDate"),

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get leave request history.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of leave request.</returns>
        public IList<Leave> GetLeaveRequestHistory(ICriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new Leave
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = reader.ToString("LeaveType"),
                                Remark = reader.ToString("Remark"),

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Get members by manager.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetMembersByManager(ICriteria criteria)
        { 
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Employee>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Adjust annual leaves.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool AdjustAnnualLeaves(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Reset annual leaves.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool ResetAnnualLeaves(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Get annual leave transactions.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of annual leave balance model.</returns>
        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<AnnualLeaveBalanceModel>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Delete annual leave transaction.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool DeleteAnnualLeaveTransaction(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Get annual leave transaction.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Annual leave balance model.</returns>
        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionByID(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<AnnualLeaveBalanceModel>(criteria.GetSpParams()).SingleOrDefault();
            }
        }

        /// <summary>
        /// Update annual leave transaction.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool UpdateAnnualLeaveTransaction(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria) != 0;
            }
        }

        /// <summary>
        /// Insert annual leave transaction.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool InsertAnnualLeaveTransaction(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Get missing check in/out employee list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of missing check in out model.</returns>
        public IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MissingCheckInOutModel>().ToList();
            }
        }

        /// <summary>
        /// Get user changed shift.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user changed shift.</returns>
        public IList<UserChangedShift> GetUserChangedShift(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<UserChangedShift>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Get half day offs.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of half day off.</returns>
        public IList<HalfDayOff> GetHalfDayOffs(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<HalfDayOff>(criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Update off day for all employees.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Output value.</returns>
        public Hashtable UpdateOffDayForAllEmployees(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            { 
                db.ExecuteNonQuery(criteria.GetSpParams());
                return db.GetOuputParams();
            }
        }

        /// <summary>
        /// Get range date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The range date.</returns>
        public int GetRangeDate(ICriteria criteria)
        {
            int numberDay = 0;
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    var ds = new DataSet();
                    ds = db.ExecuteDataSet(criteria.GetSpParams());

                    var rangeDate = ds != null ? ds.Tables[0] : null;

                    if (rangeDate != null)
                    {
                        if (rangeDate.Rows.Count > 0)
                        {
                            numberDay = int.Parse(rangeDate.Rows[0][0].ToString());
                        }
                    }
                }
            }

            return numberDay;
        }

        /// <summary>
        /// Get disable request date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of date time.</returns>
        public IEnumerable<DateTime> GetDisableRequestDate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DateTime>(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Get employee TOEIC score.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetEmployeeTOEICScore(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         e.TOEICScore = new TOEIC
                         {
                             InputDate = t.InputDate,
                             Listening = t.Listening,
                             Reading = t.Reading,
                             LRExamDate = t.LRExamDate,
                             Speaking = t.Speaking,
                             Writing = t.Writing,
                             SWExamDate = t.SWExamDate
                         };
                         return e;
                     },
                     splitOn: "TeamID",
                     parameterValues: criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        /// <summary>
        /// Function to get employees has birthday and first working.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <param name="employeesBirthday">Employees has Birthday in a date.</param>
        /// <param name="employeesFirstWorkingday">Employees has First working in a date.</param>
        public void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                    {
                        employeesBirthday = multi.Read<Employee>().ToList();
                        employeesFirstWorkingday = multi.Read<Employee>().ToList();
                    }
                }
            }
        }
    }
}
