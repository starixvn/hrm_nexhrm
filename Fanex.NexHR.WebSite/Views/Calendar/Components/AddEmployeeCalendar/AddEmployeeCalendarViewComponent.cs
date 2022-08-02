namespace Fanex.NexHR.WebSite.Views.Calendar.Components.AddEmployeeCalendar
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.ViewModels;

    public class AddEmployeeCalendarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new AddEmployeeCalendarViewModel());
        }
    }
}