using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fanex.NexHR.Models.Core;
using Fanex.NexHR.Models.Letstalk;
using Fanex.NexHR.Services.Interfaces;
using Newtonsoft.Json;

namespace Fanex.NexHR.Services;

public class LetstalkService : ILetstalkService
{
    private readonly HttpClient httpClient;
    private readonly LetstalkSettings letstalkSettings;
    private const string AuthPath = "api/authenticate";
    private const string NotifyPath = "api/notify";

    public LetstalkService(
        IHttpClientFactory httpClientFactory,
        LetstalkSettings letstalkSettings)
    {
        httpClient = httpClientFactory.CreateClient();
        this.letstalkSettings = letstalkSettings;
    }

    public async Task SendNotificationAsync(string letstalkId, string content, ReplyMarkup replyMarkup = null)
    {
        if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalkId))
        {
            var jwtToken = await GetTokenAsync(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
            if (jwtToken != null)
            {
                var data = new Dictionary<string, object>
                {
                    { "BotId", letstalkSettings.BotId },
                    { "ConversationId", letstalkId },
                    { "Message", content },
                    { "ReplyMarkup", JsonConvert.SerializeObject(replyMarkup) }
                };

                await SendNotificationToLetstalkAsync(letstalkSettings.BaseUrl, jwtToken.Token, data);
            }
        }
    }

    public void SendNotification(string letstalkId, string content, ReplyMarkup replyMarkup = null)
    {
        if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalkId))
        {
            var jwtToken = GetToken(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
            if (jwtToken != null)
            {
                var data = new Dictionary<string, object>
                {
                    { "BotId", letstalkSettings.BotId },
                    { "ConversationId", letstalkId },
                    { "Message", content },
                    { "ReplyMarkup", JsonConvert.SerializeObject(replyMarkup) }
                };

                SendNotificationToLetstalk(letstalkSettings.BaseUrl, jwtToken.Token, data);
            }
        }
    }

    private JwtToken GetToken(string baseUrl, string userName, string password)
    {
        var url = $"{baseUrl}{AuthPath}?username={userName}&password={password}";
        var response = httpClient.GetAsync(url).GetAwaiter().GetResult();

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<JwtToken>(content);
        }

        return null;
    }

    private async Task<JwtToken> GetTokenAsync(string baseUrl, string userName, string password)
    {
        var url = $"{baseUrl}{AuthPath}?username={userName}&password={password}";
        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JwtToken>(content);
        }

        return null;
    }

    private void SendNotificationToLetstalk(string baseUrl, string token, Dictionary<string, object> data)
    {
        var url = baseUrl + NotifyPath;
        var jsonContent = JsonConvert.SerializeObject(data);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
    }

    private async Task SendNotificationToLetstalkAsync(string baseUrl, string token, Dictionary<string, object> data)
    {
        var url = baseUrl + NotifyPath;
        var jsonContent = JsonConvert.SerializeObject(data);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        await httpClient.PostAsync(url, httpContent);
    }
}