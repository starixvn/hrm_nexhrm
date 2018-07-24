namespace Fanex.NexHR.HangFire.BioTask
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core.Enums;

    public interface INexbotEvent
    {
        void Initialize();

        Task SendMessageToConversation(CategoryMessageType categoryId);
    }
}