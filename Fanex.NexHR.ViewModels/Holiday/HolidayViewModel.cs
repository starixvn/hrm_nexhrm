namespace Fanex.NexHR.ViewModels.Holiday
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HolidayViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
    }
}
