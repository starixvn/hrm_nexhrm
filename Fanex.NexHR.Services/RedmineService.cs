namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using Core;
    using Core.Enums;
    using Criteria.RedmineAPI;
    using Interfaces;
    using Models;
    using Models.RedmineAPI;
    using Newtonsoft.Json;

    public class RedmineService : BaseService, IRedmineService
    {
        private readonly string applicationType = ".json";
        private readonly string baseDomain = AppSetting.Redmine_API_Address;

        private int GetCurrentRedmineId(string loginName)
        {
            int redmineId = 0;
            var criteria = new GetRedmineUserIdCriteria { LoginName = loginName };

            if (criteria.IsValid())
            {
                var userRedmind = GetDataEntity<RedmineUser>(criteria);
                redmineId = userRedmind != null ? userRedmind.RedmineId : 0;
            }

            return redmineId;
        }

        private HttpClient AuthorizeRequestHeader()
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AppSetting.Redmin_Admin_Account}")));

            var client = new HttpClient
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };

            return client;
        }

        private async Task<RedmineUser> GetCurrentRedmineUser(string domainName)
        {
            HttpClient client = AuthorizeRequestHeader();
            int currentId = GetCurrentRedmineId(domainName);
            var url = new Uri($"{baseDomain}users/{currentId}{applicationType}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            var redmineUser = new RedmineUser();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);

                redmineUser.RedmineId = obj["user"]["id"];
                redmineUser.ApiKey = obj["user"]["api_key"];
            }
            return redmineUser;
        }

        private static string GetLoginNameThroughDomainName(string domainName)
        {
            return domainName.Substring(5);
        }

        public async Task<IList<RedmineIssue>> GetInProgressIssues(string domainName)
        {
            HttpClient client = AuthorizeRequestHeader();
            int currentId = GetCurrentRedmineId(GetLoginNameThroughDomainName(domainName));
            var url = new Uri($"{baseDomain}issues.json?assigned_to_id={currentId}&status_id={(int)RedmineIssueStatus.InProgress}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            var tasks = new List<RedmineIssue>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);
                var issues = obj["issues"];

                for (int i = 0; i < obj["total_count"]; i++)
                {
                    var issue = new RedmineIssue
                    {
                        IssueId = (int)issues[i]["id"],
                        Subject = issues[i]["subject"],
                        Description = issues[i]["description"],
                        ProjectName = issues[i]["project"]["name"],
                        Author = issues[i]["author"]["name"],
                        StartDate = DateTime.ParseExact(issues[i]["start_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(issues[i]["due_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Priority = (RedmineIssuePriority)issues[i]["priority"]["id"],
                        Status = (RedmineIssueStatus)issues[i]["status"]["id"]
                    };

                    tasks.Add(issue);
                }
            }
            return tasks.OrderByDescending(x => x.Priority).ToList();
        }

        public async Task<RedmineIssue> GetIssue(int issueId)
        {
            HttpClient client = AuthorizeRequestHeader();
            var url = new Uri($"{baseDomain}issues/{issueId}{applicationType}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            var redmineIssue = new RedmineIssue();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);
                var issue = obj["issue"];

                redmineIssue.IssueId = (int)issue["id"];
                redmineIssue.Subject = issue["subject"];
                redmineIssue.Description = issue["description"];
                redmineIssue.ProjectName = issue["project"]["name"];
                redmineIssue.EstimatedHours = (double)issue["estimated_hours"];
                redmineIssue.SpentHours = (double)issue["spent_hours"];
                redmineIssue.StartDate = DateTime.ParseExact(issue["start_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                redmineIssue.DueDate = DateTime.ParseExact(issue["due_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                redmineIssue.Author = issue["author"]["name"];
                redmineIssue.Priority = (RedmineIssuePriority)issue["priority"]["id"];
                redmineIssue.Status = (RedmineIssueStatus)issue["status"]["id"];
            }

            return redmineIssue;
        }

        public async Task<CommandResult> UpdateIssueRedmine(int issueId, RedmineIssueStatus status, string domainName)
        {
            HttpClient client = new HttpClient();

            var currentUser = await GetCurrentRedmineUser(GetLoginNameThroughDomainName(domainName)).ConfigureAwait(false);
            var url = new Uri($"{baseDomain}issues/{issueId}{applicationType}?key={currentUser.ApiKey}");
            var jsonData = "{\"issue\":" +
                JsonConvert.SerializeObject(new
                {
                    status_id = status
                }) + "}";

            HttpResponseMessage response = await client.PutAsync(url,
                                                                new StringContent(jsonData, Encoding.UTF8, "application/json"))
                                                                .ConfigureAwait(false);
            var result = new CommandResult();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                result.Code = -1;
                result.Message = response.ReasonPhrase;
            }
            return result;
        }

        public async Task<CommandResult> LogtimeToIssue(LogTimeIssue issue, string domainName)
        {
            HttpClient client = new HttpClient();

            var currentUser = await GetCurrentRedmineUser(GetLoginNameThroughDomainName(domainName)).ConfigureAwait(false);
            var url = new Uri($"{baseDomain}time_entries{applicationType}?key={currentUser.ApiKey}");

            var jsonData = "{\"time_entry\":" +
                JsonConvert.SerializeObject(new
                {
                    issue_id = issue.IssueId,
                    spent_on = issue.SpentOn.ToString("yyyy-MM-dd"),
                    hours = issue.Hours,
                    comments = issue.Comments
                }) + "}";

            HttpResponseMessage response = await client.PostAsync(url,
                                                                new StringContent(jsonData, Encoding.UTF8, "application/json"))
                                                                .ConfigureAwait(false);
            var result = new CommandResult();
            if (response.StatusCode != HttpStatusCode.Created)
            {
                result.Code = -1;
                result.Message = response.ReasonPhrase;
            }
            return result;
        }
    }
}