namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IPositionService
    {
        Task<IEnumerable<Position>> GetAll();
    }
}
