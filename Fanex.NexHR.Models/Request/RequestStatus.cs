namespace Fanex.NexHR.Models.Request;

using Resources;

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