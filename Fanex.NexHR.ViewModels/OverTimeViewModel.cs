using Fanex.NexHR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Fanex.NexHR.Resources;
using Fanex.NexHR.Core.Enums;

namespace Fanex.NexHR.ViewModels
{
    public class OverTimeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Approved", ResourceType = typeof(Labels))]
        public bool IsApprove { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public float Hour { get; set; }
        public int CreatedId { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Labels))]
        public string CreatedNickName { get; set; }

        public int ApprovedId { get; set; }
        [Display(Name = "ApprovedBy", ResourceType = typeof(Labels))]
        public string ApprovedNickName { get; set; }

        public int VerifiedId { get; set; }
        [Display(Name = "VerifiedBy", ResourceType = typeof(Labels))]
        public string VerifiedNickName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeDisplayName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        public string Description { get; set; }
        public int Status { get; set; }
        public string RecordOTSheet { get; set; }

        public OverTimeViewModel()
        {
            Date = DateTime.Now;
            Hour = 2;
            Status = (int)OTRequestStatus.Pending;
            IsApprove = true;
        }

        public static OverTimeViewModel Parse(OverTime overtime)
        {
            return new OverTimeViewModel()
            {
                Id = overtime.Id,
                Date = overtime.Date,
                Hour = overtime.Hour,
                EmployeeId = overtime.Employee.Id,
                EmployeeDisplayName = string.Format("{0} ({1})", overtime.Employee.NickName, overtime.Employee.FullName),
                CreatedId = overtime.CreatedBy.Id,
                CreatedNickName = overtime.CreatedBy.NickName,
                ApprovedId = overtime.ApprovedBy != null ? overtime.ApprovedBy.Id : 0,
                ApprovedNickName = overtime.ApprovedBy != null ? overtime.ApprovedBy.NickName : string.Empty,
                VerifiedId = overtime.VerifiedBy != null ? overtime.VerifiedBy.Id : 0,
                VerifiedNickName = overtime.VerifiedBy != null ? overtime.VerifiedBy.NickName : string.Empty,
                Description = overtime.Description,
                Status = overtime.Status
            };
        }
    }
}
