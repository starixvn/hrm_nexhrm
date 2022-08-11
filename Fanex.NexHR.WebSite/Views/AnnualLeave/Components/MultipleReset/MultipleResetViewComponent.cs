namespace Fanex.NexHR.WebSite.Views.AnnualLeave.Components.MultipleReset
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class MultipleResetViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AddMultiAnnualLeaveTransactionViewModel model)
        {
            return View(model);
        }
    }
}