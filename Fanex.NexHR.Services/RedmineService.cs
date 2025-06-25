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
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Fanex.Data;
    using Interfaces;
    using Models;
    using Models.RedmineAPI;
    using Newtonsoft.Json;
    using Resources;

    public class RedmineService : IRedmineService
    {
        private readonly AppSetting appSetting;
        private readonly HttpClient httpClient;
        private readonly IBaseService baseService;
        private const string apiDateTimeFormat = "yyyy-MM-dd";

        public RedmineService(AppSetting appSetting, IHttpClientFactory httpClientFactory, IBaseService baseService)
        {
            this.appSetting = appSetting;
            httpClient = httpClientFactory.CreateClient();
            this.baseService = baseService;
        }

        public async Task<int> GetCurrentRedmineId(string loginName)
        {
            var splitName = loginName.Split('\\');
            var nickName = splitName.Length > 1 ? splitName[1] : splitName[0];
            var redmineId = 0;
            using var request = CreateRequest(string.Format("/users.json?name={0}&offset=0&limit=100", nickName), HttpMethod.Get);

            try
            {
                using var response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<dynamic>(json);
                    redmineId = (int)obj["users"][0]["id"];
                }
            }
            catch
            {
                redmineId = 0;
            }

            return redmineId;
        }

        private HttpRequestMessage CreateRequest(string endPointPath, HttpMethod method)
        {
            Uri url = new($"{appSetting.RedmineBaseUrl}{endPointPath}");
            var request = new HttpRequestMessage(method, url);
            string secret = Cryptography.DecryptPassword(appSetting.RedmineSecret);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secret}")));
            return request;
        }

        private async Task<string> GetCurrentRedmineUser(string domainName)
        {
            int currentId = await GetCurrentRedmineId(domainName);
            var apiKey = string.Empty;

            if (currentId > 0)
            {
                using var request = CreateRequest(string.Format("/users/{0}.json?offset=0&limit=100", currentId), HttpMethod.Get);

                using var response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<dynamic>(json);
                    apiKey = obj["user"]["api_key"];
                }
            }

            return apiKey;
        }

        public async Task<IList<RedmineIssue>> GetInProgressIssues(string domainName)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var tasks = new List<RedmineIssue>();

            int currentId = await GetCurrentRedmineId(domainName);

            if (currentId > 0)
            {
                string endpoint = string.Format("issues.json?assigned_to_id={0}&status_id={1}&offset=0&limit=100",
                          currentId,
                          (int)RedmineIssueStatus.InProgress);
                using var request = CreateRequest(endpoint, HttpMethod.Get);

                using var response = await httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    GenerateTaskList(tasks, json);
                }
            }

            return tasks.OrderByDescending(x => x.Priority).ToList();
        }

        private static void GenerateTaskList(List<RedmineIssue> tasks, string json)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(json);

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
                    string startDate = issue["start_date"];
                    DateTime.TryParseExact(startDate, apiDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseStartDate);
                    issueRemine.StartDate = parseStartDate.Year > 1 ? parseStartDate : null;
                }

                if (issue.ContainsKey("due_date"))
                {
                    string dueDate = issue["due_date"];
                    DateTime.TryParseExact(dueDate, apiDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDueDate);
                    issueRemine.DueDate = parseDueDate.Year > 1 ? parseDueDate : null;
                }

                tasks.Add(issueRemine);
            }
        }

        public async Task<CommandResult> LogtimeToIssue(LogTimeIssue issue, string domainName)
        {
            var result = new CommandResult();
            var apiKey = await GetCurrentRedmineUser(domainName);

            if (string.IsNullOrEmpty(apiKey))
            {
                result.Code = -1;
                result.Message = Labels.NoPermission;
            }
            else
            {
                var url = string.Format("time_entries.json?key={0}", apiKey);
                using var request = CreateRequest(url, HttpMethod.Post);

                var jsonData = "{\"time_entry\":" +
                    JsonConvert.SerializeObject(new
                    {
                        issue_id = issue.IssueId,
                        spent_on = issue.SpentOn.ToString(apiDateTimeFormat),
                        hours = issue.Hours,
                        comments = issue.Comments,
                        activity_id = (int)issue.Activity
                    }) + "}";
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                using var response = await httpClient.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    result.Code = -1;
                    result.Message = response.ReasonPhrase;
                }
            }

            return result;
        }

        public async Task<IList<TimeEntry>> GetTimeEntries(int currentUserId, DateTime fromDate, DateTime toDate)
        {
            var listEntries = new List<TimeEntry>();

            await GetTimeEntriesRecursiveAsync(listEntries, currentUserId, 0, fromDate, toDate);

            return listEntries;
        }

        public async Task GetTimeEntriesRecursiveAsync(List<TimeEntry> entries, int currentId, int offset, DateTime fromDate, DateTime toDate)
        {
            const int limit = 100;
            string url = string.Format("time_entries.json?user_id={0}&spent_on=><{1}|{2}&offset={3}&limit={4}",
                currentId,
                fromDate.ToString(apiDateTimeFormat),
                toDate.ToString(apiDateTimeFormat),
                offset,
                limit);
            using var request = CreateRequest(url, HttpMethod.Get);

            using var response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<dynamic>(json);
                var timeEntries = obj["time_entries"];
                var totalItems = (int)obj["total_count"];

                foreach (var timeEntry in timeEntries)
                {
                    string spentTimeDate = timeEntry["spent_on"];
                    DateTime.TryParseExact(spentTimeDate, apiDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime spentDate);

                    var entry = new TimeEntry
                    {
                        SpentOn = spentDate,
                        Hours = (double)timeEntry["hours"],
                    };

                    entries.Add(entry);
                }

                offset += limit;

                if (offset <= totalItems)
                {
                    await GetTimeEntriesRecursiveAsync(entries, currentId, offset, fromDate, toDate);
                }
            }
        }

        private async Task<IEnumerable<DateTime>> GetWorkedDatesOfUser(DateTime from, DateTime to, int currentUserId)
        {
            var criteria = new GetWorkedDatesByUserCriteria
            {
                UserId = currentUserId,
                FromDate = from,
                ToDate = to
            };
            return await baseService.GetDataAsync<DateTime>(criteria);
        }

        public async Task<IList<DateTime>> GetHaveNotLogTimeDates(string domainUser, int currentUserId)
        {
            var toDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().AddDays(-1);
            var fromDate = toDate.AddDays(-6);
            var currentUser = await GetCurrentRedmineId(domainUser);

            if (currentUser > 0)
            {
                var timeEntries = await GetTimeEntries(currentUser, fromDate, toDate);

                var hadLogtimeDates = timeEntries.GroupBy(x => x.SpentOn).Select(d => d.Key).ToList();
                var onWorkedDates = await GetWorkedDatesOfUser(fromDate, toDate, currentUserId);

                return onWorkedDates.Except(hadLogtimeDates).OrderBy(x => x).ToList();
            }
            return new List<DateTime>();
        }

        public async Task<double> GetTotalSpentHoursOnDate(string domainName, DateTime onDate)
        {
            double spentHours = 0;
            DateTime toDate = onDate;
            var currentUserId = await GetCurrentRedmineId(domainName);
            if (currentUserId > 0)
            {
                var timeEntries = await GetTimeEntries(currentUserId, onDate, toDate);
                spentHours = timeEntries.Sum(x => x.Hours);
            }

            return spentHours;
        }
    }
}