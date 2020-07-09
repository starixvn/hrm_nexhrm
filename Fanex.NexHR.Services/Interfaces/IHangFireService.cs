namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IHangFireService
    {
        Task<bool> ObjectIsChangedOrNotExists(int categoryId, object value);

        Task<string> GetJsonObject(int categoryId);

        Task<CommandResult> UpdateJsonObject(int categoryId, object value);
    }
}