namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum ProfessionalLevel
    {
        [StringValue("Fresher")]
        Fresher = 1,

        [StringValue("Engineer")]
        Engineer = 2,

        [StringValue("Senior")]
        Senior = 3,

        [StringValue("Uper Senior")]
        UperSenior = 4
    }
}