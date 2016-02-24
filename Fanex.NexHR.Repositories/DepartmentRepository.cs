namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using System.Linq;

    public class DepartmentRepository : IDepartmentRepository
    {
        public IList<Department> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Department, Employee, Employee, Employee, Department>(
                    (de, cre, man, modi) =>
                    {
                        de.Creator = cre;
                        de.Manager = man;
                        de.Modifier = modi;
                        return de;
                    },
                    "CreatedBy, ManagerId, ModifiedBy",
                    criteria.GetSpParams()).ToList();
            }
        }

        public Department GetDepartmentById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Department, Employee, Employee, Employee, Department>(
                    (dep, cre, mag, modi) =>
                    {
                        dep.Creator = cre;
                        dep.Manager = mag;
                        dep.Modifier = modi;
                        return dep;
                    },
                    "CreatedBy, ManagerId, ModifedBy",
                    criteria.GetSpParams());
            }
        }
    }
}
