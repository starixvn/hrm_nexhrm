namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IHangFireService
    {
        Task<bool> ObjectIsChangedOrNotExists(int categoryId, object value);

        Task<string> GetJsonObject(int categoryId);

        Task UpdateJsonObject(int categoryId, object value);
    }
}