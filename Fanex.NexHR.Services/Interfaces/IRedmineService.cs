namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Fanex.NexHR.Models.RedmineAPI;
    using Models;

    public interface IRedmineService
    {
        Task<IList<RedmineIssue>> GetInProgressIssues(string domainName);

        Task<RedmineIssue> GetIssue(int issueId);

        Task<CommandResult> UpdateIssueRedmine(int issueId, RedmineIssueStatus status, string domainName);

        Task<CommandResult> LogtimeToIssue(LogTimeIssue issue, string domainName);
    }
}