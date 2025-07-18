namespace Fanex.NexHR.Models.Letstalk;

using System;

[Serializable]
public class JwtToken
{
    public string Token { get; set; }
    public DateTime ValidTo { get; set; }
}