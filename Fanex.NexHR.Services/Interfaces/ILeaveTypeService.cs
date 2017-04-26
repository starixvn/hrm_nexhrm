namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Models;

    public interface ILeaveTypeService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        IList<LeaveType> GetAll();
    }
}
