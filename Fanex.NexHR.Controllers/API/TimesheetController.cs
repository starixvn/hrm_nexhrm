namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class TimesheetController : ApiController
    {
        private readonly ITimeSheetService timeSheetService;

        public TimesheetController()
            : this(null)
        {
        }

        public TimesheetController(ITimeSheetService timeSheetService)
        {
            this.timeSheetService = timeSheetService ?? new TimeSheetService();
        }

        [HttpGet]
        public IHttpActionResult ReminderReachedWorkingTime()
        {
            timeSheetService.SendReminderReachedWorkingTime();
            return Ok();
        }
    } 
}