// --------------------------------------------------------------------------
// <copyright file="RequestTypeModel.cs" company=".">
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
    /// Request type.
    /// </summary>
    public class RequestTypeModel : EntityBase<short>
    {
        const int number2 = 2;
        const int number3 = 3;
        const int number4 = 4;
        const int number5 = 5;
        const int number6 = 6;

        public new string Name
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return Labels.CheckIn;
                    case number2:
                        return Labels.CheckOut;
                    case number3:
                        return Labels.LateIn;
                    case number4:
                        return Labels.EarlyOut;
                    case number5:
                        return Labels.ChangeShift;
                    case number6:
                        return Labels.Leave;
                    default:
                        return string.Empty;
                }
            }
        }

        public int RequestFilter
        {
            get
            {
                switch (Id)
                {
                    case 1:
                    case number2:
                        return number4;
                    case number3:
                    case number4:
                        return number3;
                    case number5:
                        return number2;
                    case number6:
                        return 1;
                    default:
                        return number5;
                }
            }
        }

        public int RequestHistoryFilter
        {
            get
            {
                switch (Id)
                {
                    case 1:
                    case number2:
                        return 1;
                    case number3:
                    case number4:
                        return number2;
                    case number5:
                        return number3;
                    case number6:
                        return number4;
                    default:
                        return 0;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}