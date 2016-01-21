using Fanex.NexHR.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Core.Enums
{
    public enum OTRequestStatus : int
    {
        [Description("New")]
        New = 1,

        [Description("Approved")]
        Approved,

        [Description("Verified")]
        Verified
    }
}
