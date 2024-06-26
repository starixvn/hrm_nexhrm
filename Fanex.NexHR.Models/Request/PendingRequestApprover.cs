namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;

    public class PendingRequestApprover
    {
        public long RequestId { get; set; }
        public int EmpId { get; set; }
        public string Requester { get; set; }
        public string FullNameRequester { get; set; }
        public string FromShift { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public RequestType RequestType { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Remark { get; set; }
        public int ApproverId { get; set; }
        public string Approver { get; set; }
        public string ApproverFullName { get; set; }
        public string ApproverConversationId { get; set; }
    }
}