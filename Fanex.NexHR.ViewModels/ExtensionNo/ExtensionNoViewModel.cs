namespace Fanex.NexHR.ViewModels.ExtensionNo
{
    using System.Collections.Generic;
    using Models;

    public class ExtensionNoViewModel
    {
        public string Name { get; set; }

        public IList<ExtensionNoModel> ListExt { get; set; }
    }
}