using Fanex.NexHR.Models.Letstalk;

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
        private readonly ILetstalkService letstalkService;

        public TestController(
            INexbotService botService,
            ILetstalkService letstalkService
        )
        {
            this.botService = botService;
            this.letstalkService = letstalkService;
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

        [HttpGet]
        public async Task<IActionResult> SendMessageLetstalk()
        {
            var replyMarkup = new ReplyMarkup
            {
                Actions =
                [
                    new() {
                        ActionType = ReplyActionType.PostBack,
                        Label = "Accept",
                        Data = "callback_accept"
                    },
                    new() {
                        ActionType = ReplyActionType.PostBack,
                        Label = "Reject",
                        Data = "callback_reject"
                    },
                    new() {
                        ActionType = ReplyActionType.Uri,
                        Label = "View details",
                        Uri = "https://www.google.com"
                    }
                ]
            };
            await letstalkService.SendNotificationAsync("t870342617", "test", replyMarkup);
            return Ok();
        }
    }
}