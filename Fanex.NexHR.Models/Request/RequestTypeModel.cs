namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core.Enums;
    using Resources;

    public class RequestTypeModel : EntityBase<short>
    {
        public new string Name
        {
            get
            {
                switch (Id)
                {
                    case (short)RequestType.CheckIn:
                        return Labels.CheckIn;
                    case (short)RequestType.CheckOut:
                        return Labels.CheckOut;
                    case (short)RequestType.Leave:
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
                    case (short)RequestType.CheckIn:
                    case (short)RequestType.CheckOut:
                        return (int)RequestTypeFilterForRequest.CheckInOut;
                    case (short)RequestType.Leave:
                        return (int)RequestTypeFilterForRequest.Leave;
                    default:
                        return (int)RequestTypeFilterForRequest.All;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}