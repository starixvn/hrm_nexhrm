namespace Fanex.NexHR.Models.NexBot
{
    using System;
    using System.Configuration;

    public static class NexBotConfiguration
    {
        public static Uri BotServiceUrl { get; internal set; }
            = new Uri(ConfigurationManager.AppSettings["FanexBotClient:BotServiceUrl"]);

        public static string ClientId { get; internal set; }
            = ConfigurationManager.AppSettings["MicrosoftAppId"] ??
                "1fca3d04-4799-4544-8744-89367a838238";

        public static string ClientPassword { get; internal set; }
            = ConfigurationManager.AppSettings["MicrosoftAppPassword"] ??
                "NJT594-!uoobpdkYYBF29:^";

        public static Uri TokenUrl { get; internal set; }
            = new Uri(ConfigurationManager.AppSettings["TokenUrl"] ??
                "https://login.microsoftonline.com/botframework.com/oauth2/v2.0/token");
    }
}