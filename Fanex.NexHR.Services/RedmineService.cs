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
    using Criteria.Employee;
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
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AppSetting.Redmine_Admin_Account}")));

            var client = new HttpClient
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };

            return client;
        }

        private async Task<string> GetCurrentRedmineUser(string domainName)
        {
            HttpClient client = AuthorizeRequestHeader();
            int currentId = GetCurrentRedmineId(domainName);
            var url = new Uri($"{baseDomain}users/{currentId}{applicationType}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            var apiKey = string.Empty;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);

                apiKey = obj["user"]["api_key"];
            }
            return apiKey;
        }

        private static string GetLoginNameByDomainName(string domainName)
        {
            return domainName.Substring(5);
        }

        public async Task<IList<RedmineIssue>> GetInProgressIssues(string domainName)
        {
            HttpClient client = AuthorizeRequestHeader();
            int currentId = GetCurrentRedmineId(GetLoginNameByDomainName(domainName));
            var url = new Uri($"{baseDomain}issues{applicationType}?assigned_to_id={currentId}&status_id={(int)RedmineIssueStatus.InProgress}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            var tasks = new List<RedmineIssue>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);

                foreach (var issue in obj["issues"])
                {
                    var issueRemine = new RedmineIssue
                    {
                        IssueId = (int)issue["id"],
                        Subject = issue["subject"],
                        Description = issue["description"],
                        ProjectName = issue["project"]["name"],
                        Priority = issue["priority"]["name"],
                        Tracker = issue["tracker"]["name"]
                    };

                    tasks.Add(issueRemine);
                }
            }
            return tasks.OrderByDescending(x => x.Priority).ToList();
        }

        public async Task<CommandResult> LogtimeToIssue(LogTimeIssue issue, string domainName)
        {
            HttpClient client = new HttpClient();
            var result = new CommandResult();

            var apiKey = await GetCurrentRedmineUser(GetLoginNameByDomainName(domainName)).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(apiKey))
            {
                result.Code = -1;
                result.Message = "No Permission";
            }
            else
            {
                var url = new Uri($"{baseDomain}time_entries{applicationType}?key={apiKey}");

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
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    result.Code = -1;
                    result.Message = response.ReasonPhrase;
                }
            }

            return result;
        }

        private async Task<IList<TimeEntry>> GetTimeEntries(string domainUser, DateTime fromDate, DateTime toDate)
        {
            HttpClient client = AuthorizeRequestHeader();
            var currentUserId = GetCurrentRedmineId(GetLoginNameByDomainName(domainUser));
            var listEntries = new List<TimeEntry>();

            var url = new Uri($"{baseDomain}time_entries{applicationType}?user_id={currentUserId}&spent_on=><{fromDate.ToString("yyyy-MM-dd")}|{toDate.ToString("yyyy-MM-dd")}");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var obj = jss.Deserialize<dynamic>(json);
                var timeEntries = obj["time_entries"];

                foreach (var timeEntry in timeEntries)
                {
                    var entry = new TimeEntry
                    {
                        SpentOn = DateTime.ParseExact(timeEntry["spent_on"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Hours = (double)timeEntry["hours"],
                        Activity = (RedmineLogTimeActivity)timeEntry["activity"]["id"]
                    };

                    listEntries.Add(entry);
                }
            }

            return listEntries;
        }

        private IList<DateTime> GetWorkedDatesOfUser(DateTime from, DateTime to, int currentUserId)
        {
            var criteria = new GetWorkedDatesByUserCriteria
            {
                UserId = currentUserId,
                FromDate = from,
                ToDate = to
            };
            return GetData<DateTime>(criteria);
        }

        public async Task<IList<DateTime>> GetHaveNotLogTimeDates(string domainUser, int currentUserId)
        {
            var toDate = DateTime.Now.AddDays(-1);
            var fromDate = toDate.AddDays(-6);
            var timeEntries = await GetTimeEntries(domainUser, fromDate, toDate).ConfigureAwait(false);
            var hadLogtimeDates = timeEntries.GroupBy(x => x.SpentOn).Select(d => d.Key).ToList();
            var onWorkedDates = GetWorkedDatesOfUser(fromDate, toDate, currentUserId).ToList();

            return onWorkedDates.Except(hadLogtimeDates).OrderBy(x => x).ToList();
        }
    }
}