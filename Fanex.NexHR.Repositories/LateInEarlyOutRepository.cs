namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Repositories.Interfaces;

    public class LateInEarlyOutRepository : ILateInEarlyOutRepository
    {
        public async Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(ICriteria criteria)
        {
            IEnumerable<LateInEarlyOutDiscipline> lateinEarlyOuts;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    lateinEarlyOuts = await multi.ReadAsync<LateInEarlyOutDiscipline>();

                    var lateInEarlyOutDetails = await multi.ReadAsync<LateInEarlyOutDisciplineDetail>();

                    foreach (var lateinEarlyOut in lateinEarlyOuts)
                    {
                        lateinEarlyOut.Details = lateInEarlyOutDetails.Where(e => e.EmpId == lateinEarlyOut.EmpId);
                    }
                }
            }

            return lateinEarlyOuts;
        }
    }
}