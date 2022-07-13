namespace Fanex.NexHR.Models.NexBot
{
    public class NexBotSettings
    {
        public string BotServiceUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientPassword { get; set; }
        public string TokenUrl { get; set; }
        public bool DisabledSendSkype { get; set; }
    }
}