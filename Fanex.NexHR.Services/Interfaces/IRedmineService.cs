namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Models.RedmineAPI;

    public interface IRedmineService
    {
        Task<IList<RedmineIssue>> GetInProgressIssues(string domainName);

        Task<CommandResult> LogtimeToIssue(LogTimeIssue issue, string domainName);

        Task<IList<DateTime>> GetHaveNotLogTimeDates(string domainUser, int currentUserId);
    }
}