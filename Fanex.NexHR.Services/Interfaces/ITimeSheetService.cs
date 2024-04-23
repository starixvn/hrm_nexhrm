namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ITimeSheetService
    {
        Task SendReminderReachedWorkingTime();

        Task ImportDataFromBioStar();

        Task NotifyEmployeeAfterCheckOut();
    }
}