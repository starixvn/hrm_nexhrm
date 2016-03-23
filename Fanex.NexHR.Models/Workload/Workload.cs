namespace Fanex.NexHR.Models.Workload
{
    using System;

    public class Workload
    {
        public Employee Employee { get; set; }

        public double PercentWorkload { get; set; }

        public string DisplayPercentWorkload
        {
            get
            {
                string display = string.Empty;
                display = (Math.Round(PercentWorkload, 2) * 100) + "%";

                return display;
            }
        }

        public string CssClassForPercent 
        {
            get
            {
                string css = string.Empty;
                double roundedPercent = Math.Round(PercentWorkload, 2);
                if (roundedPercent >= 0.9)
                {
                    css = "light-blue lighten-1";
                }
                else if (roundedPercent >= 0.5 && roundedPercent < 0.9)
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
