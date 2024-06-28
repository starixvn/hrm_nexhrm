namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ApproverRequestSumup
    {
        public int ApproverId { get; set; }
        public string ApproverNickname { get; set; }
        public string ConversationId { get; set; }
        public int TotalRequest => Requests.Count();
        public string CallbackUrl { get; set; }

        public string RequestIds
        {
            get
            {
                return string.Join(",", Requests.Select(i => i.RequestId));
            }
        }

        public IEnumerable<MemberRequest> Requests { get; set; }
    }

    public class MemberRequest
    {
        public bool IsLeaveRequest { get; set; }
        public int ApproverId { get; set; }
        public long RequestId { get; set; }
        public string NickName { get; set; }
        public string TeamName { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string TotalDays { get; set; }
        public string RequestType { get; set; }
        public string FromDate { get; set; }
        public string Remark { get; set; }
        public string DateDetails { get; set; }
        public string CallbackUrl { get; set; }
        public string CreatedDate { get; set; }
    }
}