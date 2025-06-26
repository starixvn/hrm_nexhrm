using Fanex.NexHR.Criteria.Holiday;
using Fanex.NexHR.Models.Holiday;
using Fanex.NexHR.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using Fanex.NexHR.Core;
using Fanex.NexHR.Services.Interfaces;
using Newtonsoft.Json;

namespace Fanex.NexHR.Services.AutoJobService;

public class BotMealService(IHolidayService holidayService,
    IHttpClientFactory httpClientFactory,
    AppSetting setting,
    IBaseService baseService,
    INexbotService nexBotService) : IBotMealService
{
    private const int ItemEachBatch = 10;

    public async Task SendCancelMealMessage()
    {
        var validDate = await GetNextValidMealDate();
        if (validDate == null)
        {
            return;
        }

        var employees = await GetEmployeesByValidDate(validDate.Value);

        var employeeConversationCriteria = new Criteria.Employee.GetEmployeeConversationCriteria
        {
            EmployeeJson = JsonConvert.SerializeObject(employees),
            OnDate = validDate.Value
        };
        var employeeConversations = await baseService.GetDataAsync<EmployeeMealModel>(employeeConversationCriteria);
        employeeConversations = employeeConversations.Where(e => e.NickName == "Henry" || e.NickName == "Hank" || e.NickName == "George").ToList();
        if (employeeConversations.Any())
        {
            await SendMessagesByBatches(employeeConversations);
        }
    }

    private async Task SendMessagesByBatches(IEnumerable<EmployeeMealModel> employeeConversations)
    {
        var batches = new List<List<EmployeeMealModel>>();

        var totalBatch = (int)Math.Ceiling((double)employeeConversations.Count() / ItemEachBatch);
        for (var i = 0; i < totalBatch; i++)
        {
            batches.Add(employeeConversations.Skip(i * ItemEachBatch).Take(ItemEachBatch).ToList());
        }

        foreach (var batch in batches)
        {
            var tasks = new List<Task>();
            foreach (var employeeConversation in batch)
            {
                tasks.Add(nexBotService.SendRemindCancelMeal(employeeConversation));
            }

            await Task.WhenAll(tasks);
        }
    }

    private async Task<List<EmployeeMealModel>> GetEmployeesByValidDate(DateTime validDate)
    {
        List<EmployeeMealModel> employees = [];
        var client = httpClientFactory.CreateClient(Constants.Fnb_Client);
        var requestBody = new
        {
            OnDate = validDate
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, Constants.Application_Json);

        var response = await client.PostAsync($"{setting.FBUrl}/api/{Endpoint.Get_Employees_Meal_Cancel_Register}", content);

        if (!response.IsSuccessStatusCode)
        {
            return employees;
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return employees;
        }
        employees = JsonConvert.DeserializeObject<List<EmployeeMealModel>>(responseContent);

        return employees;
    }

    private async Task<DateTime?> GetNextValidMealDate()
    {
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var holidayCriteria = new GetHolidayByYearCriteria
        {
            Year = currentDate.Year
        };
        var holidays = (await holidayService.GetByYear(holidayCriteria)).ToList();
        var isOffDay = CheckOffDay(currentDate, holidays);
        if (isOffDay)
        {
            return null;
        }
        while (true)
        {
            var nextDay = currentDate.AddDays(1);
            var isNextDayInvalid = CheckOffDay(nextDay, holidays);
            if (!isNextDayInvalid)
            {
                return nextDay;
            }

            currentDate = nextDay;
        }
    }

    private static bool CheckOffDay(DateTime date, IEnumerable<Holiday> holidays)
    {
        var isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var isHoliday = holidays.Any(h => date >= h.FromDate && date <= h.ToDate);
        return isWeekend || isHoliday;
    }
}