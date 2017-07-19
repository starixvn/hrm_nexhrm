namespace Fanex.NexHR.Repositories
{
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.CompanyEvent;
    using Models.Workload;
    using System.Collections.Generic;
    using System.Linq;

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
                     "TeamID, JobTitleId, LRExamDate",
                     criteria.GetSpParams());

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
                    model.EventList = multi.Read<CompanyEvent>().ToList();
                }
            }

            return model;
        }

        public List<Workload> GetWorkloadByTeam(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Workload, Employee, Workload>(
                     (w, e) =>
                     {
                         w.Employee = e;
                         return w;
                     },
                     parameterSet: criteria.GetSpParams());

                return entityList.ToList();
            }
        }

        private static Employee GetEmployeeEntity(ICriteria criteria)
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

        private static DefaultSchedule GetDefaultSchedule(dynamic defaultSchedule)
        {
            var def = new DefaultSchedule
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

        private static Shift GetMonShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                    Id = defaultSchedule.MonShiftId ?? 0,
                    Name = defaultSchedule.MonShiftName
                };
        }

        private static Shift GetTueShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.TueShiftId ?? 0,
                Name = defaultSchedule.TueShiftName
            };
        }

        private static Shift GetWedShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.WedShiftId ?? 0,
                Name = defaultSchedule.WedShiftName
            };
        }

        private static Shift GetThuShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.ThuShiftId ?? 0,
                Name = defaultSchedule.ThuShiftName
            };
        }

        private static Shift GetFriShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.FriShiftId ?? 0,
                Name = defaultSchedule.FriShiftName
            };
        }

        private static Shift GetSatShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.SatShiftId ?? 0,
                Name = defaultSchedule.SatShiftName
            };
        }

        private static Shift GetSunShift(dynamic defaultSchedule)
        {
            return new Shift
            {
                Id = defaultSchedule.SunShiftId ?? 0,
                Name = defaultSchedule.SunShiftName
            };
        }
    }
}