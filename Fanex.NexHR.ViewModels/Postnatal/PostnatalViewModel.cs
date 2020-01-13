namespace Fanex.NexHR.ViewModels.Postnatal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Resources;

    public class PostnatalViewModel
    {
        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Fullname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int OldEmpId { get; set; }

        public DateTime OldFromDate { get; set; }

        public DateTime OldToDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int Minutes { get; set; }

        public int Types { get; set; }

        [Display(Name = "Type")]
        [Required]
        public List<int> ChosenTypes { get; set; }

        public List<SelectListItem> TypeList
        {
            get
            {
                const int secondCase = 2;
                const int threeCase = 3;

                List<SelectListItem> listTypes = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = Labels.LateInEarlyOut,
                        Value = "1"
                    },
                    new SelectListItem
                    {
                        Text = Labels.BreakTime,
                        Value = "2"
                    }
                };
                if (Types != 0)
                {
                    switch (Types)
                    {
                        case secondCase:
                            listTypes[1].Selected = true;
                            break;
                        case threeCase:
                            listTypes[0].Selected = true;
                            listTypes[1].Selected = true;
                            break;
                        default:
                            listTypes[0].Selected = true;
                            break;
                    }
                }

                return listTypes;
            }
        }
    }
}