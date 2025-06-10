using System;

namespace BIO.WebAPI.Models
{
    public class RedmineTimeEntry
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public DateTime SpentOn { get; set; }
        public double Hours { get; set; }
    }
}