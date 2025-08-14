using System.Threading.Tasks;
using Fanex.NexHR.Models.Letstalk;

namespace Fanex.NexHR.Services.Interfaces;

public interface ILetstalkService
{
    Task SendNotificationAsync(string letstalkId, string content, ReplyMarkup replyMarkup = null);
}