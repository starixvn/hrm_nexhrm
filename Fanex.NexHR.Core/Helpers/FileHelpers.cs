using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Core.Services;
using System.Threading.Tasks;

namespace Fanex.NexHR.Core.Helpers;

public static class FileHelpers
{
    private static IMinIOService minIOService;

    public static void Initialize(IMinIOService minIOServices)
    {
        minIOService = minIOServices;
    }

    public static async Task<string> GetAvatar(string userId, string defaultAvatar = "default-avatar.jpg")
    {
        if (minIOService == null)
        {
            return string.Empty;
        }

        var avatarName = string.IsNullOrEmpty(userId) ? "default" : userId + ".jpg";
        var avatarPath = $"1/{avatarName}";
        var avatarUrl = await minIOService.GetFilePath(Bucket.Avatar, avatarPath, defaultAvatar);
        return avatarUrl;
    }
}