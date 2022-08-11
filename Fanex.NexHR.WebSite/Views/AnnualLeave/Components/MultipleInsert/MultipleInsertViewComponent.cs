namespace Fanex.NexHR.WebSite.Views.AnnualLeave.Components.MultipleInsert
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class MultipleInsertViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AddMultiAnnualLeaveTransactionViewModel model)
        {
            return View(model);
        }
    }
}