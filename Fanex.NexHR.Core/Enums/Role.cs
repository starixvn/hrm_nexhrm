namespace Fanex.NexHR.Core.Enums
{
    using Fanex.NexHR.Core.Attributes;

    public enum Role : int
    {
        /// <summary>
        /// Member role.
        /// </summary>
        [StringValue("Member")]
        Member = 0,

        /// <summary>
        /// Assistant leader role.
        /// </summary>
        [StringValue("AssistantLeader")]
        AssistantLeader = 1,

        /// <summary>
        /// Leader role.
        /// </summary>
        [StringValue("Leader")]
        Leader = 2,

        /// <summary>
        /// HR department role.
        /// </summary>
        [StringValue("HR")]
        HR = 3,

        /// <summary>
        /// System admin role.
        /// </summary>
        [StringValue("SA")]
        SA = 4,

        /// <summary>
        /// Project Manager role.
        /// </summary>
        [StringValue("PM")]
        PM = 5
    }
}