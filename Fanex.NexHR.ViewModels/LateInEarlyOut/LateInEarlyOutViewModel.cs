namespace Fanex.NexHR.ViewModels.LateInEarlyOut
{
    using System;

    public class LateInEarlyOutViewModel
    {
        public int Id { get; set; }

        public string Nickname { get; set; }

        public string Type { get; set; }

        public DateTime WorkDate { get; set; }

        public int Minutes { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
