// --------------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.23">Created</change>
//   <change who="Anders.Le" date="2012.5.23">Modify Get function</change>
//   <change who="Bradley.Mac" date="2014.3.7">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class EmployeeRepository : IEmployeeRepository
    {
        public Employee GetById(ICriteria criteria)
        {
            return GetEmployeeEntity(criteria);
        }

        public Employee GetByDomainName(ICriteria criteria)
        {
            return GetEmployeeEntity(criteria);
        }

        public IList<Employee> Query(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, dynamic, Toeic, Employee>(
                     (e, t, toeic) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         e.ToeicScore = toeic;
                         return e;
                     },
                     "TeamID, EmpID",
                     criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        public MySpaceViewModel GetMySpace(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    MySpaceViewModel dashboard = multi.Read<MySpaceViewModel>().FirstOrDefault();

                    IEnumerable<DashboardPersonalSpaceModel> dashboardCompanySpace = multi.Read<DashboardPersonalSpaceModel, Employee, DashboardPersonalSpaceModel>(
                        (co, e) =>
                        {
                            co.Employee = e;
                            return co;
                        },
                        "EmpId");

                    IEnumerable<DashboardPersonalSpaceModel> dashboardEmployeeSpace = multi.Read<DashboardPersonalSpaceModel, Employee, DashboardPersonalSpaceModel>(
                        (pe, e) =>
                        {
                            pe.Employee = e;
                            return pe;
                        },
                        "EmpId");

                    dashboard.DashboardCompanySpace = dashboardCompanySpace.ToList();
                    dashboard.DashboardEmployeeSpace = dashboardEmployeeSpace.ToList();

                    return dashboard;
                }
            }
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var ds = new DataSet();
                var date = criteria.ReportDate;

                ds = db.ExecuteDataSet(criteria.GetSpParams());
                var dttWorkingSchedule = ds != null ? ds.Tables[0] : null;

                var lstWorkingSchedule = new List<WorkingScheduleModel>();
                if (dttWorkingSchedule.Rows.Count != 0)
                {
                    for (int i = 1; i <= dttWorkingSchedule.Columns.Count - 4; i++)
                    {
                        var workingDay = new WorkingScheduleModel
                        {
                            Title = dttWorkingSchedule.Rows[0][i + 3].ToString(),
                            Start = new DateTime(date.Year, date.Month, i).ToShortDateString()
                        };
                        lstWorkingSchedule.Add(workingDay);
                    }
                }

                return lstWorkingSchedule;
            }
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var lstWorkingCalendar = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                ds = db.ExecuteDataSet(criteria.GetSpParams());
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

                    lstWorkingCalendar.Add(empWorkingCalendar);
                }

                return lstWorkingCalendar;
            }
        }

        public bool UpdateShift(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        public bool ResetAnnualLeaves(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

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
        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(ICriteria criteria)
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
                return db.Query<MissingCheckInOutModel>(criteria.GetSpParams()).ToList();
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
        public IList<Employee> GetEmployeeToeicScore(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.JobTitle = new JobTitle { Id = t.JobId, Name = t.JobTitle };
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         e.ToeicScore = new Toeic
                         {
                             InputDate = t.InputDate,
                             Listening = t.Listening,
                             Reading = t.Reading,
                             LRExamDate = t.LRExamDate,
                             Speaking = t.Speaking,
                             Writing = t.Writing,
                             SWExamDate = t.SWExamDate,
                         };
                         return e;
                     },
                     splitOn: "JobId",
                     parameterValues: criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        /// <summary>
        /// Function to get employees has birthday and first working.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Tuple List Employee has birthday and employee has first working day.</returns>
        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria)
        {
            var model = new SystemReminderModel();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    model.EmployeeHasBirthday = multi.Read<Employee>().ToList();
                    model.EmployeeHasFirstWorkingDay = multi.Read<Employee>().ToList();
                }
            }

            return model;
        }

        /// <summary>
        /// Function to get all related people in updating TOEIC score.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of related people.</returns>
        public IList<Employee> GetUpdatingScoreRelatedPeople(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Employee>(criteria.GetSpParams()).ToList();
            }
        }

        private Employee GetEmployeeEntity(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employee = db.QueryEntity<Employee, dynamic, Toeic, Team, JobTitle, Employee>(
                    (e, def, to, te, job) =>
                    {
                        e.DefaultSchedule = GetDefaultSchedule(def);
                        e.ToeicScore = to;
                        e.Team = te;
                        e.JobTitle = job;
                        return e;
                    },
                    "MonShiftId, LRExamDate, GroupId, JobtitleId",
                    criteria.GetSpParams());

                return employee;
            }
        }

        private DefaultSchedule GetDefaultSchedule(dynamic defaultSchedule)
        {
            var def = new DefaultSchedule()
            {
                Monday = new Shift()
                {
                    Id = defaultSchedule.MonShiftId == null ? 0 : defaultSchedule.MonShiftId,
                    Name = defaultSchedule.MonShiftName
                },
                Tuesday = new Shift()
                {
                    Id = defaultSchedule.TueShiftId == null ? 0 : defaultSchedule.TueShiftId,
                    Name = defaultSchedule.TueShiftName
                },
                Wednesday = new Shift()
                {
                    Id = defaultSchedule.WedShiftId == null ? 0 : defaultSchedule.WedShiftId,
                    Name = defaultSchedule.WedShiftName
                },
                Thursday = new Shift()
                {
                    Id = defaultSchedule.ThuShiftId == null ? 0 : defaultSchedule.ThuShiftId,
                    Name = defaultSchedule.ThuShiftName
                },
                Friday = new Shift()
                {
                    Id = defaultSchedule.FriShiftId == null ? 0 : defaultSchedule.FriShiftId,
                    Name = defaultSchedule.FriShiftName
                },
                Saturday = new Shift()
                {
                    Id = defaultSchedule.SatShiftId == null ? 0 : defaultSchedule.SatShiftId,
                    Name = defaultSchedule.SatShiftName
                },
                Sunday = new Shift()
                {
                    Id = defaultSchedule.SunShiftId == null ? 0 : defaultSchedule.SunShiftId,
                    Name = defaultSchedule.SunShiftName
                },
            };

            return def;
        }
    }
}