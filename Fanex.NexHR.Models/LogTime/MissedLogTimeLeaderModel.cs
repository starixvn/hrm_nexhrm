using System.Collections.Generic;

namespace Fanex.NexHR.Models.LogTime
{
    public class MissedLogTimeLeaderModel
    {
        public string ConversationId { get; set; }
        public string LeaderName { get; set; }
        public List<MemberModel> Members { get; set; }
    }


    public class MemberModel
    {
        public string NickName { get; set; }

        public int MissedCount { get; set; }
    }
}