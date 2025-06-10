namespace Fanex.NexHR.Controllers;

using System.Threading.Tasks;
using Core;
using Core.Helpers;
using Criteria.Employee;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

public class AvatarController(
    IWebHostEnvironment environment,
    IEmployeeService employeeService,
    AppSetting appSetting)
    : Controller
{
    [AllowAnonymous]
    [Route("Avatars/{id}")]
    public async Task<IActionResult> Avatar(string id)
    {
        var avatarUrl = await FileHelpers.GetAvatar(id);
        return Content(avatarUrl, "text/plain");
    }

    [AllowAnonymous]
    [Route("avatar/{imgNickName}")]
    public async Task<IActionResult> GetAvatar(string imgNickName)
    {
        var index = imgNickName.LastIndexOf('.');
        var nickName = imgNickName.EndsWith(".jpg") ? imgNickName[..index] : imgNickName;
        var domainName = $"{appSetting.DomainName}{nickName}";
        var employee = await employeeService.GetEmployeeByDomainName(new GetEmployeeByDomainNameCriteria
            { DomainUserName = domainName });

        var avatarUrl = await FileHelpers.GetAvatar(employee?.Code);
        return Content(avatarUrl, "text/plain");
    }
}