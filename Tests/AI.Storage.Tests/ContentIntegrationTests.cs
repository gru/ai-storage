using System.Net.Http.Headers;
using AI.Storage.Entities;
using AI.Storage.Http.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Xunit;
using NSubstitute;
using Amazon.S3;
using Amazon.S3.Model;

namespace AI.Storage.Tests;

public class ContentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IContentV1Controller _client;
    private readonly IAmazonS3 _mockS3Client;

    public ContentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _mockS3Client = Substitute.For<IAmazonS3>();

        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StorageDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContextPool<StorageDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Replace the real IAmazonS3 with our mock
                services.AddSingleton<IAmazonS3>(_ => _mockS3Client);
            });
        });

        var httpClient = testFactory.CreateClient();
        _client = RestClient.For<IContentV1Controller>(httpClient);

        // Setup mock responses for S3 client
        _mockS3Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PutObjectResponse()));

        _mockS3Client.GetObjectAsync(Arg.Any<GetObjectRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var response = new GetObjectResponse
                {
                    ResponseStream = new MemoryStream("This is a test file content"u8.ToArray()),
                    Headers = { ContentType = "text/plain" }
                };
                return Task.FromResult(response);
            });
    }

    private static MultipartFormDataContent CreateTestFileContent(string fileName, string content)
    {
        var multipartContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        multipartContent.Add(fileContent, "file", fileName);
        return multipartContent;
    }

    [Fact]
    public async Task CreateContent_ShouldUploadFileSuccessfully()
    {
        // Arrange
        var fileName = "test.txt";
        var content = "This is a test file content";
        var multipartContent = CreateTestFileContent(fileName, content);

        // Act
        var response = await _client.CreateContent(multipartContent, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ContentIds);
        await _mockS3Client.Received().PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetContent_ShouldDownloadFileSuccessfully()
    {
        // Arrange
        var fileName = "test.txt";
        var content = "This is a test file content";
        var multipartContent = CreateTestFileContent(fileName, content);

        var createResponse = await _client.CreateContent(multipartContent, CancellationToken.None);
        var contentId = createResponse.ContentIds.First();

        // Act
        var response = await _client.GetContent(contentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.ToString());
        Assert.Equal(fileName, response.Content.Headers.ContentDisposition?.FileName);

        var downloadedContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(content, downloadedContent);
        await _mockS3Client.Received().GetObjectAsync(Arg.Any<GetObjectRequest>(), Arg.Any<CancellationToken>());
    }
}