namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;

    public class DepartmentRepository : IDepartmentRepository
    {
        public async Task<IList<Department>> GetAll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var departments = await db.QueryAsync<Department, Employee, Employee, Employee, Department>(
                    (de, cre, man, modi) =>
                    {
                        de.Creator = cre;
                        de.Manager = man;
                        de.Modifier = modi;
                        return de;
                    },
                    "CreatedBy, ManagerId, ModifiedBy", criteria);

                return departments.ToList();
            }
        }

        public async Task<Department> GetDepartmentById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryEntityAsync<Department, Employee, Employee, Employee, Department>(
                    (dep, cre, mag, modi) =>
                    {
                        dep.Creator = cre;
                        dep.Manager = mag;
                        dep.Modifier = modi;
                        return dep;
                    },
                    "CreatedBy, ManagerId, ModifedBy", criteria);
            }
        }
    }
}
