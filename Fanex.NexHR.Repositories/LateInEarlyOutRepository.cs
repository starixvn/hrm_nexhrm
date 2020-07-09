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

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria.GetSpParams()).ConfigureAwait(false))
                {
                    lateinEarlyOuts = await multi.ReadAsync<LateInEarlyOutDiscipline>().ConfigureAwait(false);

                    var lateInEarlyOutDetails = await multi.ReadAsync<LateInEarlyOutDisciplineDetail>().ConfigureAwait(false);

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