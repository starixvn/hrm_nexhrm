using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Fanex.Logging;
using Fanex.NexHR.Core.Model;
using Microsoft.AspNetCore.Http;

namespace Fanex.NexHR.Core.Services;

public interface IMinIOService
{
    Task<bool> UploadFileAsync(IFormFile file, string bucketName, string filePath);

    Task<Stream> GetFileContentAsync(string bucketName, string filePath);

    Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "");

    Task<bool> DeleteFileAsync(string bucketName, string objectName);

    Task DeleteDirectoryAsync(string bucketName, string directory);

    Task<bool> IsFileExistsAsync(string bucketName, string filePath);
}

public class MinIOService : IMinIOService, IDisposable
{
    private readonly AmazonS3Client client;
    private readonly AmazonStorageConfig s3Config;
    private readonly ILogger logger;
    private bool disposed;

    public MinIOService(AmazonStorageConfig s3Config, ILogger logger)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = s3Config.InternalUrl,
            ForcePathStyle = true,
            UseHttp = true
        };

        client = new AmazonS3Client(s3Config.AccessKey, s3Config.SecretKey, config);
        this.logger = logger;
        this.s3Config = s3Config;
    }

    public async Task<bool> UploadFileAsync(IFormFile file, string bucketName, string filePath)
    {
        try
        {
            await using var fileStream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = filePath,
                InputStream = fileStream
            };

            var response = await client.PutObjectAsync(putRequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            await logger.ErrorAsync($"Upload to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Upload to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
    }

    public async Task<bool> IsFileExistsAsync(string bucketName, string filePath)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = filePath,
            MaxKeys = 1
        };

        var response = await client.ListObjectsV2Async(request);
        return response.S3Objects?.Exists(o => o.Key == filePath) ?? false;
    }

    public async Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "")
    {
        var isExistFile = await IsFileExistsAsync(bucketName, filePath);
        var fileName = isExistFile ? filePath : defaultFile;

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = fileName,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Verb = HttpVerb.GET
        };
        return await client.GetPreSignedURLAsync(request);
    }

    public async Task<Stream> GetFileContentAsync(string bucketName, string filePath)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = filePath
        };

        using var response = await client.GetObjectAsync(request);
        var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Reset stream position
        return memoryStream;
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string objectName)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = RemoveFirstSlash(objectName)
            };

            var response = await client.DeleteObjectAsync(deleteRequest);
            return response.HttpStatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex.Message);
            return false;
        }
    }

    public async Task DeleteDirectoryAsync(string bucketName, string directory)
    {
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = $"{directory}/"
            };

            var response = await client.ListObjectsV2Async(request);

            response.S3Objects.ForEach(async obj => await client.DeleteObjectAsync(bucketName, obj.Key));
        }
        catch (AmazonS3Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex.Message);
        }
    }

    private static string RemoveFirstSlash(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return filename;
        }

        return filename.StartsWith('/') ? filename[1..] : filename;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            client?.Dispose();
        }

        disposed = true;
    }

    ~MinIOService()
    {
        Dispose(false);
    }
}