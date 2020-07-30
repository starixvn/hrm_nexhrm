namespace Fanex.NexHR.ViewModels.Employee
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Models;

    public class WorkingHistoryViewModel
    {
        public WorkingHistoryViewModel()
        {
            ToDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            AffectedDate = DateTime.Now;
        }
        public int? Id { get; set; }

        public int EmpId { get; set; }
     
        public int ModifiedBy { get; set; }

        public string JobtitleName { get; set; }

        public string GroupName { get; set; }

        public int JobTitleId { get; set; }

        public int GroupId { get; set; }

        public int GroupIdOld { get; set; }

        [DataType(DataType.Date)]
        public DateTime AffectedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ModifiedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        
        public DateTime ToDate { get; set; }

        public IList<HistoryWork> HistoryWorkes { get; set; }

        public Employee Employee { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<JobTitle> JobTitles { get; set; }

        public IList<SelectListItem> GetSelectListItemTeam(int? teamId)
        {
            IList<SelectListItem> selectListItemTeam = new List<SelectListItem>();

            var firstItem = new SelectListItem
            {
                Value = string.Empty,
                Selected = true
            };

            selectListItemTeam.Add(firstItem);

            Teams = Teams ?? new List<Team>();

            foreach (Team team in Teams)
            {
                var item = new SelectListItem
                {
                    Value = team.Id.ToString(),
                    Text = team.Name,
                    Selected = teamId == team.Id
                };

                selectListItemTeam.Add(item);
            }

            return selectListItemTeam;
        }
    }
}