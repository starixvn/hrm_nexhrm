namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;

    public interface INexbotService
    {
        Task SendMessage(string conversationId, string message);

        Task<IEnumerable<string>> GetRegisteredConversation(HangfireCategoryMessageType categoryId);
    }
}