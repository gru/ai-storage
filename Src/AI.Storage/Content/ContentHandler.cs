using System.Net.Http.Headers;
using System.Net.Mime;
using AI.Storage.Entities;
using AI.Storage.Http.Contracts;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IO;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AI.Storage.Content;

/// <summary>
/// Handles business logic operations related to Contents.
/// This class is responsible for creating and retrieving Content entities.
/// </summary>
public class ContentHandler
{
   private readonly ProjectDbContext _dbContext;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    
    private static readonly RecyclableMemoryStreamManager _streamManager = new(
        blockSize: 1024 * 16,
        largeBufferMultiple: 1024 * 1024,
        maximumBufferSize: 1024 * 1024 * 16,
        useExponentialLargeBuffer: true,
        maximumSmallPoolFreeBytes: 1024 * 1024 * 64,
        maximumLargePoolFreeBytes: 1024 * 1024 * 128);

    /// <summary>
    /// Initializes a new instance of the ContentHandler class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing Content entities.</param>
    /// <param name="s3Client">The Amazon S3 client for interacting with S3 storage.</param>
    /// <param name="configuration">The configuration to access app settings.</param>
    public ContentHandler(ProjectDbContext dbContext, IAmazonS3 s3Client, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
        _bucketName = configuration["AWS:BucketName"] ?? throw new InvalidOperationException("S3 bucket name is not configured.");
    }

    /// <summary>
    /// Creates new Content entities based on the files uploaded in the HTTP request.
    /// </summary>
    /// <param name="httpContext">The HttpContext containing the multipart request.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A CreateContentResponse containing the IDs of the newly created Content entities.</returns>
    public async Task<CreateContentResponse> CreateContent(HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (IsMultipartContentType(httpContext.Request.ContentType))
            throw new InvalidOperationException("Not a multipart request.");

        var ids = new List<long>();
        var reader = new MultipartReader(httpContext.Request.GetMultipartBoundary(), httpContext.Request.Body);
        var section = await reader.ReadNextSectionAsync(cancellationToken);
        
        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
            if (hasContentDispositionHeader && contentDisposition!.DispositionType.Equals("form-data") && !string.IsNullOrEmpty(contentDisposition.FileName))
            {
                var fileName = contentDisposition.FileName;
                var contentType = section.ContentType;

                var blobKey = $"{Guid.NewGuid()}";
                using (var memoryStream = _streamManager.GetStream("S3UploadBuffer"))
                {
                    await section.Body.CopyToAsync(memoryStream, cancellationToken);
                    
                    memoryStream.Position = 0;

                    var putObjectRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = blobKey,
                        InputStream = memoryStream,
                        ContentType = contentType
                    };

                    await _s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
                }

                var contentEntity = new ContentEntity
                {
                    FileName = fileName,
                    ContentType = contentType ?? MediaTypeNames.Application.Octet,
                    BlobKey = blobKey
                };

                _dbContext.Contents.Add(contentEntity);
                
                await _dbContext.SaveChangesAsync(cancellationToken);

                ids.Add(contentEntity.Id);
            }

            section = await reader.ReadNextSectionAsync(cancellationToken);
        }

        return new CreateContentResponse
        {
            ContentIds = ids
        };
    }

    /// <summary>
    /// Retrieves and downloads a Content entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A FileStreamResult containing the downloaded content.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a Content with the specified ID is not found.</exception>
    public async Task<FileStreamResult> GetContent(long id, CancellationToken cancellationToken)
    {
        var content = await _dbContext.Contents.FindAsync([id], cancellationToken);
            
        if (content == null)
        {
            throw new InvalidOperationException($"Content with id {id} not found");
        }

        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = content.BlobKey
        };

        var response = await _s3Client.GetObjectAsync(getObjectRequest, cancellationToken);

        return new FileStreamResult(response.ResponseStream, content.ContentType)
        {
            FileDownloadName = content.FileName
        };
    }
    
    private static bool IsMultipartContentType(string? contentType)
    {
        return !string.IsNullOrEmpty(contentType) && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
    }
}
