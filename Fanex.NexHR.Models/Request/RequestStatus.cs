// --------------------------------------------------------------------------
// <copyright file="RequestStatus.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using Resources;

    /// <summary>
    /// Request status.
    /// </summary>
    public class RequestStatus : EntityBase<byte>
    {
        public new string Name
        {
            get
            {
                const int secondCase = 2;
                const int threeCase = 3;

                switch (Id)
                {
                    case 1:
                        return Labels.Pending;
                    case secondCase:
                        return Labels.Approved;
                    case threeCase:
                        return Labels.Rejected;
                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
