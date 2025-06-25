using System.Threading.Tasks;

namespace Fanex.NexHR.Services.Interfaces;

public interface IBotMealService
{
    Task SendCancelMealMessage();
}