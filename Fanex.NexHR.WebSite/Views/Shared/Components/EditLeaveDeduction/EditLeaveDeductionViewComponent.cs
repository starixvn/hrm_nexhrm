namespace Fanex.NexHR.WebSite.Views.Shared.Components.AnnualLeave.EditLeaveDeduction
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class EditLeaveDeductionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LeaveDeductionViewModel model)
        {
            return View(model);
        }
    }
}