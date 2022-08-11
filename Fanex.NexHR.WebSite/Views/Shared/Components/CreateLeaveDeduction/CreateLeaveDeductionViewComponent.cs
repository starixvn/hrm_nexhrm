namespace Fanex.NexHR.WebSite.Views.Shared.Components.CreateLeaveDeduction
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class CreateLeaveDeductionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LeaveDeductionViewModel model)
        {
            return View(model);
        }
    }
}