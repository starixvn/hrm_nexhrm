namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    public interface IMessageCategoryLogService
    {
        Task<bool> ObjectIsChangedOrNotExists(MessageCategory categoryId, object value);

        Task<string> GetJsonObject(MessageCategory categoryId);

        Task<CommandResult> UpdateJsonObject(MessageCategory categoryId, object value);
    }
}