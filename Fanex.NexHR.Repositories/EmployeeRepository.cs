namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.CompanyEvent;
    using Models.Workload;

    public class EmployeeRepository : IEmployeeRepository
    {
        public async Task<Employee> GetById(ICriteria criteria)
        {
            return await GetEmployeeEntity(criteria);
        }

        public async Task<Employee> GetByNickName(ICriteria criteria)
        {
            return await GetEmployeeEntity(criteria);
        }

        public async Task<IList<Employee>> Query(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<Employee, Toeic, Team, JobTitle, Employee>(
                     (employee, toeic, team, jobTitle) =>
                     {
                         employee.Team = team;
                         employee.JobTitle = jobTitle;
                         employee.ToeicScore = toeic;
                         return employee;
                     },
                     "LRExamDate, GroupId, JobTitleId",
                     criteria);

                return entityList.ToList();
            }
        }

        public async Task<IList<DashBoardPersonalSpace>> GetDashBoardPersonalSpace(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<DashBoardPersonalSpace, Employee, DashBoardPersonalSpace>(
                    (pe, e) =>
                    {
                        pe.Employee = e;
                        return pe;
                    },
                    "EmpId",
                    criteria);

                return entityList.ToList();
            }
        }

        public async Task<IList<DashBoardCompanySpace>> GetDashBoardCompanySpace(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<DashBoardCompanySpace, Employee, DashBoardCompanySpace>(
                    (pe, e) =>
                    {
                        pe.Employee = e;
                        return pe;
                    },
                    "EmpId",
                    criteria);

                return entityList.ToList();
            }
        }

        public async Task<IList<Employee>> GetEmployeeToeicScore(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<Employee, Team, JobTitle, Toeic, Employee>(
                     (e, t, job, to) =>
                     {
                         e.JobTitle = job;
                         e.Team = t;
                         e.ToeicScore = to;
                         return e;
                     },
                     "TeamID, JobTitleId, LRExamDate",
                     criteria);

                return entityList.ToList();
            }
        }

        public async Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria)
        {
            var model = new SystemReminderModel();
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    model.EmployeeHasBirthday = multi.Read<Employee>().ToList();
                    model.EmployeeHasFirstWorkingDay = multi.Read<Employee>().ToList();
                    model.EventList = multi.Read<CompanyEvent>().ToList();
                }
            }

            return model;
        }

        public async Task<List<Workload>> GetWorkloadByTeam(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<Workload, Employee, Workload>(
                     (w, e) =>
                     {
                         w.Employee = e;
                         return w;
                     },
                     splitOn: "Id",
                     parameterSet: criteria);

                return entityList.ToList();
            }
        }

        private static async Task<Employee> GetEmployeeEntity(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var employee = await db.QueryEntityAsync<Employee, dynamic, Toeic, Team, JobTitle, Employee>(
                    (e, def, to, te, job) =>
                    {
                        e.DefaultSchedule = GetDefaultSchedule(def);
                        e.ToeicScore = to;
                        e.Team = te;
                        e.JobTitle = job;
                        return e;
                    },
                    "MonShiftId, LRExamDate, GroupId, JobtitleId",
                    criteria);

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