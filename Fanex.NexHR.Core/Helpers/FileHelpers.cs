using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Core.Model;
using Fanex.NexHR.Core.Services;
using System.Threading.Tasks;

namespace Fanex.NexHR.Core.Helpers;

public static class FileHelpers
{
    private static IMinIOService minIOService;
    private static AmazonStorageConfig s3Config;

    public static void Initialize(IMinIOService minIOServices, AmazonStorageConfig s3Configs)
    {
        minIOService = minIOServices;
        s3Config = s3Configs;
    }

    public static async Task<string> GetAvatar(string userId, string defaultAvatar = "default-avatar.jpg")
    {
        if (minIOService == null)
        {
            return string.Empty;
        }

        var avatarName = string.IsNullOrEmpty(userId) ? "default" : userId + ".jpg";
        var avatarPath = $"1/{avatarName}";
        var avatarUrl = await minIOService.GetFilePath(StorageInfo.Avatar.Bucket, avatarPath, defaultAvatar);
        return avatarUrl;
    }

    public static string GetResumeDir()
    {
        return $"{s3Config.ExternalUrl}/{StorageInfo.Resume.Dir}";
    }

    public static string GetCertificatesDir()
    {
        return $"{s3Config.ExternalUrl}/{StorageInfo.Certificates.Dir}";
    }
}