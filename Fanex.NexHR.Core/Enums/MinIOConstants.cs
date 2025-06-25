namespace Fanex.NexHR.Core.Enums;

public static class Bucket
{
    public const string File = "hrm-file";
    public const string Avatar = "hrm-avatar";
    public const string Payroll = "hrm-payroll";
}

public class Storage
{
    public string Bucket { get; set; }
    public string Path { get; set; }
    public string Dir => Bucket + "/" + Path;
}

public static class StorageInfo
{
    public static readonly Storage Resume = new() { Bucket = Bucket.File, Path = "1/resume" };
    public static readonly Storage Avatar = new() { Bucket = Bucket.Avatar, Path = "1" };
    public static readonly Storage Payroll = new() { Bucket = Bucket.Payroll, Path = "1/payroll" };
    public static readonly Storage Certificates = new() { Bucket = Bucket.File, Path = "1/certificates" };
}