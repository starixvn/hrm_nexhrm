namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Criteria.Employee;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var criteria = new GetEmployeeByUsernameCriteria
            {
                Username = username
            };

            var result = await employeeService.GetEmployeeByUsername(criteria);

            return result == null ? Ok("Not Found") : Ok(result);
        }
    }
}