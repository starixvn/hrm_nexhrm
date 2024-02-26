namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Criteria;

   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PromotionTransferController : ControllerBase
    {
        private readonly IBaseService baseService;

        public PromotionTransferController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IActionResult> UpdateTeamAndGrade()
        {
            var criteria = new UpdateTeamAndGradeCriteria();
            await baseService.ExecuteNonQueryAsync(criteria);
            return Ok();
        }
    }
}