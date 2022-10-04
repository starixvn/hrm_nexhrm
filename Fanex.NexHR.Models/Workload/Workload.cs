namespace Fanex.NexHR.Models.Workload
{
    using System;

    public class Workload
    {
        private const int NumberPercent = 100;
        public Employee Employee { get; set; }
        public double PercentWorkload { get; set; }
        public int WorkHour { get; set; }
        public string DisplayPercentWorkload => string.Format("{0:0}%", PercentWorkload * NumberPercent);

        public string CssClassForPercent
        {
            get
            {
                const double round = 0.9;
                const double round2 = 0.5;
                const double round3 = 0.9;

                string css;
                double roundedPercent = Math.Round(PercentWorkload, 2);
                if (roundedPercent >= round)
                {
                    css = "light-blue lighten-1";
                }
                else if (roundedPercent >= round2 && roundedPercent < round3)
                {
                    css = "pink darken-1";
                }
                else
                {
                    css = "grey darken-1";
                }

                return css;
            }
        }
    }
}