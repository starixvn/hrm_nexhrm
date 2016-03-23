namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NexHR.Models;
using Fanex.NexHR.Models.Workload;
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

        public IList<Employee> GetEmployeeToeicScore(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, Team, JobTitle, Toeic, Employee>(
                     (e, t, job, to) =>
                     {
                         e.JobTitle = job;
                         e.Team = t;
                         e.ToeicScore = to;
                         return e;
                     },
                     splitOn: "TeamID, JobTitleId, LRExamDate",
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

        public List<Workload> GetWorkloadByTeam(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Workload, Employee, Team, Workload>(
                     (w, e, t) =>
                     {
                         w.Employee = e;
                         e.Team = t;
                         return w;
                     },
                     splitOn: "Id, GroupId",
                     parameterValues: criteria.GetSpParams());

                return entityList.ToList();
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
                Monday = GetMonShift(defaultSchedule),
                Tuesday = GetTueShift(defaultSchedule),
                Wednesday = GetWedShift(defaultSchedule),
                Thursday = GetThuShift(defaultSchedule),
                Friday = GetFriShift(defaultSchedule),
                Saturday = GetSatShift(defaultSchedule),
                Sunday = GetSunShift(defaultSchedule)
            };

            return def;
        }

        private Shift GetMonShift(dynamic defaultSchedule)
        {
            return new Shift()
                {
                    Id = defaultSchedule.MonShiftId == null ? 0 : defaultSchedule.MonShiftId,
                    Name = defaultSchedule.MonShiftName
                };
        }

        private Shift GetTueShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.TueShiftId == null ? 0 : defaultSchedule.TueShiftId,
                Name = defaultSchedule.TueShiftName
            };
        }

        private Shift GetWedShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.WedShiftId == null ? 0 : defaultSchedule.WedShiftId,
                Name = defaultSchedule.WedShiftName
            };
        }

        private Shift GetThuShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.ThuShiftId == null ? 0 : defaultSchedule.ThuShiftId,
                Name = defaultSchedule.ThuShiftName
            };
        }

        private Shift GetFriShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.FriShiftId == null ? 0 : defaultSchedule.FriShiftId,
                Name = defaultSchedule.FriShiftName
            };
        }

        private Shift GetSatShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.SatShiftId == null ? 0 : defaultSchedule.SatShiftId,
                Name = defaultSchedule.SatShiftName
            };
        }

        private Shift GetSunShift(dynamic defaultSchedule)
        {
            return new Shift()
            {
                Id = defaultSchedule.SunShiftId == null ? 0 : defaultSchedule.SunShiftId,
                Name = defaultSchedule.SunShiftName
            };
        }
    }
}