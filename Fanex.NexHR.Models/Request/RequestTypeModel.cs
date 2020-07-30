namespace Fanex.NexHR.Models.Request
{
    using Resources;

    public class RequestTypeModel : EntityBase<short>
    {
        public new string Name
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return Labels.CheckIn;
                    case 2:
                        return Labels.CheckOut;
                    case 3:
                        return Labels.LateIn;
                    case 4:
                        return Labels.EarlyOut;
                    case 5:
                        return Labels.ChangeShift;
                    case 6:
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
                    case 2:
                        return 4;
                    case 3:
                    case 4:
                        return 3;
                    case 5:
                        return 2;
                    case 6:
                        return 1;
                    default:
                        return 5;
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
                    case 2:
                        return 1;
                    case 3:
                    case 4:
                        return 2;
                    case 5:
                        return 3;
                    case 6:
                        return 4;
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