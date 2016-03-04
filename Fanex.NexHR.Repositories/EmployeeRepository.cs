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
                var entityList = db.Query<Employee, Toeic, Team, JobTitle, Employee>(
                     (employee, toeic, team, jobTitle) =>
                     {
                         employee.Team = team;
                         employee.JobTitle = jobTitle;
                         employee.ToeicScore = toeic;
                         return employee;
                     },
                     "LRExamDate, GroupId, JobTitleId",
                     criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        public IList<DashBoardPersonalSpace> GetDashBoardPersonalSpace(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<DashBoardPersonalSpace, Employee, DashBoardPersonalSpace>(
                    (pe, e) =>
                    {
                        pe.Employee = e;
                        return pe;
                    },
                    "EmpId",
                    criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        public IList<DashBoardCompanySpace> GetDashBoardCompanySpace(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<DashBoardCompanySpace, Employee, DashBoardCompanySpace>(
                    (pe, e) =>
                    {
                        pe.Employee = e;
                        return pe;
                    },
                    "EmpId",
                    criteria.GetSpParams());

                return entityList.ToList();
            }
        }

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