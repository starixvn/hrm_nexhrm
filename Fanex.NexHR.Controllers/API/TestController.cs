namespace Fanex.NexHR.Controllers.API;

using System;
using System.Threading.Tasks;
using Fanex.NexHR.Core;
using Fanex.NexHR.Models;
using Fanex.NexHR.Models.LateInEarlyOut;
using Fanex.NexHR.Models.Letstalk;
using Fanex.NexHR.Resources;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
public class TestController(
    INexbotService botService,
    ILetstalkService letstalkService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> MissingCheckInCheckOut()
    {
        var request = new MissingCheckInOutEmployee
        {
            EmpId = 6553,
            FullName = "Đinh Quang Hoàng",
            NickName = "Lucian",
            ConversationId = "B08HJT3E2D8:T08ACMHMMFS:D08HBDU0UQ6",
            AttendanceDate = DateTimeOffset.Now.DateTime,
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
                new ReplyAction {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Accept",
                    Data = "callback_accept"
                },
                new ReplyAction {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Reject",
                    Data = "callback_reject"
                },
                new ReplyAction {
                    ActionType = ReplyActionType.Uri,
                    Label = "View details",
                    Uri = "https://www.google.com"
                }
            ]
        };
        await letstalkService.SendNotificationAsync("t870342617", "test", replyMarkup);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> SendMessageViolateOnMonth()
    {
        var violater = new LateInEarlyOutViolateModel
        {
            EmpId = 6553,
            NickName = "Lucian",
            TimeOnMonth = 10
        };
        var msg = string.Format(Messages.DearSkypeMessage, violater.NickName, Constants.NewLine);
        if (violater.TimeOnMonth >= 3)
        {
            msg += string.Format(
                Messages.EmployeeLateInEarlyOutViolateOnMonth,
                violater.TimeOnMonth,
                DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("MMM, yyyy"),
                Constants.NewLine);
        }
        else
        {
            msg = string.Format(
                Messages.EmployeeLateInEarlyOutViolateOnMonthOnceMoreTime,
                violater.NickName);
        }
        await botService.SendMessageAsync("B08HJT3E2D8:T08ACMHMMFS:D08HBDU0UQ6", msg).ConfigureAwait(false);
        return Ok();
    }
}