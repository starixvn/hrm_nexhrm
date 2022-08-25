namespace Fanex.NexHR.Models.Memo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class Memo : EntityBase<int>
    {
        [Required]
        public string Contents { get; set; }

        [Required]
        public string RefNo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string SentTo { get; set; }

        [Required]
        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        public int MemoType { get; set; }

        public string AttachFile { get; set; }

        public string CategoryName { get; set; }

        public int CheckForEdit { get; set; }

        public bool SendMail { get; set; }

        public int Status { get; set; }

        public IEnumerable<LookupData> MemoCategories { get; set; }

        public List<SelectListItem> StatusTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.All, Value = "-1" },
            new SelectListItem { Text = Labels.Active, Value = "0" },
            new SelectListItem { Text = Labels.Deactive, Value = "1" }
        };
    }
}