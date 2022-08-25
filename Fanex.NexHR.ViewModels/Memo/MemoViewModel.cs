namespace Fanex.NexHR.ViewModels.Memo
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Resources;

    public class MemoViewModel
    {
        public string Content { get; set; }

        public int Year { get; set; }

        public int Category { get; set; }

        public int Status { get; set; }

        public IEnumerable<LookupData> MemoCategories { get; set; }

        public List<SelectListItem> StatusTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.All, Value = "-1" },
            new SelectListItem { Text = Labels.Active, Value = "1" },
            new SelectListItem { Text = Labels.Deactive, Value = "0" }
        };
    }
}