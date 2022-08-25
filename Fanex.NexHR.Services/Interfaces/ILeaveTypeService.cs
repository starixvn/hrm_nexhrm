namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Models;

    public interface ILeaveTypeService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        Task<IEnumerable<LeaveType>> GetAll();
    }
}
