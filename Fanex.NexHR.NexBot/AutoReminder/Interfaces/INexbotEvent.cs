namespace Fanex.NexHR.NexBot.Interfaces
{
    using System.Threading.Tasks;
    using Core.Enums;

    public interface INexbotEvent
    {
        void Initialize();

        Task SendMessageToConversation(HangfireCategoryMessageType categoryId);
    }
}