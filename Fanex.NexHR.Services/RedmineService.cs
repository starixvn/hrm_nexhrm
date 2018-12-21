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
    using Resources;

    public class RedmineService : BaseService, IRedmineService
    {
        private readonly string apiDateTimeFormat = "yyyy-MM-dd";
        private readonly string getCurrentUserUrl = AppSetting.Redmine_API_Address + "/users/{0}.json?offset=0&limit=100";
        private readonly string getInprogessTasksUrl = AppSetting.Redmine_API_Address + "issues.json?assigned_to_id={0}&status_id=" + (int)RedmineIssueStatus.InProgress + "&offset=0&limit=100";
        private readonly string logtimeToIssueUrl = AppSetting.Redmine_API_Address + "time_entries.json?key={0}";
        private readonly string getTimeEntriesUrl = AppSetting.Redmine_API_Address + "time_entries.json?user_id={0}&spent_on=><{1}|{2}&offset=0&limit=100";

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
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AppSetting.RedmineConnectionString}")));

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
            var url = new Uri(string.Format(getCurrentUserUrl, currentId));
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
            var url = new Uri(string.Format(getInprogessTasksUrl, currentId));
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
                        Tracker = issue["tracker"]["name"],
                        Status = issue["status"]["name"],
                        CreatedBy = issue["author"]["name"],
                        Assignee = issue.ContainsKey("assigned_to") ? issue["assigned_to"]["name"] : string.Empty
                    };

                    if (issue.ContainsKey("start_date"))
                    {
                        issueRemine.StartDate = DateTime.ParseExact(issue["start_date"], apiDateTimeFormat, CultureInfo.InvariantCulture);
                    }

                    if (issue.ContainsKey("due_date"))
                    {
                        issueRemine.DueDate = DateTime.ParseExact(issue["due_date"], apiDateTimeFormat, CultureInfo.InvariantCulture);
                    }

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
            if (string.IsNullOrEmpty(apiKey))
            {
                result.Code = -1;
                result.Message = Labels.NoPermission;
            }
            else
            {
                var url = new Uri(string.Format(logtimeToIssueUrl, apiKey));

                var jsonData = "{\"time_entry\":" +
                    JsonConvert.SerializeObject(new
                    {
                        issue_id = issue.IssueId,
                        spent_on = issue.SpentOn.ToString(apiDateTimeFormat),
                        hours = issue.Hours,
                        comments = issue.Comments,
                        activity_id = (int)issue.Activity
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

            var url = new Uri(string.Format(getTimeEntriesUrl, currentUserId, fromDate.ToString(apiDateTimeFormat), toDate.ToString(apiDateTimeFormat)));
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
                        SpentOn = DateTime.ParseExact(timeEntry["spent_on"], apiDateTimeFormat, CultureInfo.InvariantCulture),
                        Hours = (double)timeEntry["hours"],
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

        public async Task<double> GetTotalSpentHoursOnDate(string domainName, DateTime onDate)
        {
            var timeEntries = await GetTimeEntries(domainName, onDate, onDate).ConfigureAwait(false);
            return timeEntries.Sum(x => x.Hours);
        }
    }
}