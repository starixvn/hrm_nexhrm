using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Fanex.NexHR.ViewModels
{
    public class PostnatalViewModel
    {        
        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int OldEmpId { get; set; }

        public DateTime OldFromDate { get; set; }

        public DateTime OldToDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int Minutes { get; set; }

        public int Type { get; set; }

        [Required]
        public List<int> Types { get; set; }
        
        public List<SelectListItem> TypeList
        {
            get
            {
                List<SelectListItem> listTypes = new List<SelectListItem>()
                {
                    new SelectListItem() 
                    {
                        Text = "Late in/Early out", 
                        Value = "1"
                    },
                    new SelectListItem() 
                    {
                        Text = "Break time", 
                        Value = "2"
                    }
                };
                if (Type != 0)
                {
                    switch (Type)
                    {
                        case 1:
                            listTypes[0].Selected = true;
                            break;
                        case 2:
                            listTypes[1].Selected = true;
                            break;
                        case 3:
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

            set 
            {
            }
        }
    }
}
