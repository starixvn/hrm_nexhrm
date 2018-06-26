namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.Bio;

    public interface IBioRepository
    {
        IEnumerable<BioTimeChange> GetAllBioTimeChanges(ICriteria criteria);
    }
}
