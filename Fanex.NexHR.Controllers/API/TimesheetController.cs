namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;

    [Authorize]
    [JwtAuthentication]
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
            LogFileHelper.WriteInfo("Call API ReminderReachedWorkingTime");

            timeSheetService.SendReminderReachedWorkingTime();
            return Ok();
        }
    } 
}