namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        private readonly INexbotService botService;

        public TestController(
            INexbotService botService
        )
        {
            this.botService = botService;
        }


        [HttpPost]
        public async Task<IActionResult> MissingCheckInCheckOut()
        {
            var request = new MissingCheckInOutEmployee
            {
                EmpId = 6553,
                FullName = "Đinh Quang Hoàng",
                NickName = "Lucian",
                ConversationId = "B08HJT3E2D8:T08ACMHMMFS:D08HBDU0UQ6",
                AttendanceDate = DateTime.Now,
                ShiftCode = "M1",
                Type = 1
            };
            await botService.SendRemindCheckInCheckOutAsync(request).ConfigureAwait(false);
            return Ok();
        }
    }
}