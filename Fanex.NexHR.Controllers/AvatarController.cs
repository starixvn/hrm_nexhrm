namespace Fanex.NexHR.Controllers;

using System.Threading.Tasks;
using Core;
using Core.Helpers;
using Criteria.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

public class AvatarController(
    IEmployeeService employeeService,
    AppSetting appSetting) : Controller
{
    [AllowAnonymous]
    [Route("Avatars/{id}")]
    public async Task<IActionResult> Avatar(string id)
    {
        var index = id.LastIndexOf('.');
        var newId = id.EndsWith(".jpg") ? id[..index] : id;
        var avatarUrl = await FileHelpers.GetAvatar(newId);
        return Redirect(avatarUrl);
    }

    [AllowAnonymous]
    [Route("avatar/{imgNickName}")]
    public async Task<IActionResult> GetAvatar(string imgNickName)
    {
        var index = imgNickName.LastIndexOf('.');
        var nickName = imgNickName.EndsWith(".jpg") ? imgNickName[..index] : imgNickName;
        var domainName = $"{appSetting.DomainName}{nickName}";
        var criteria = new GetEmployeeByDomainNameCriteria
        {
            DomainUserName = domainName
        };
        var employee = await employeeService.GetEmployeeByDomainName(criteria);

        var avatarUrl = await FileHelpers.GetAvatar(employee?.Code);
        return Redirect(avatarUrl);
    }
}