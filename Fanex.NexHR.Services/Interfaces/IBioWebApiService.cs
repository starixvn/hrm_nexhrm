namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Models.LogTime;

    public interface IBioWebApiService
    {
        Task<List<TimeEntryModel>> GetTimeEntriesAsync(DateTime date);
    }
}