namespace Fanex.NexHR.ViewModels.LateInEarlyOut
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LateInEarlyOutViewModel : BaseRequestModel
    {        
        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Type { get; set; }

        public int TypeCode { get; set; }

        public DateTime WorkDate { get; set; }

        public int Minutes { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<Types> TypesList 
        { 
            get 
            {
                return new List<Types>()
                {
                    new Types()
                    {
                        Id = 1, 
                        Name = "Late in"
                    },
                    new Types()
                    { 
                        Id = 2, 
                        Name = "Early out" 
                    }
                };
            }            
        }
    }
}
