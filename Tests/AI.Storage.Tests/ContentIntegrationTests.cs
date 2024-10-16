using System.Net.Http.Headers;
using AI.Storage.Entities;
using AI.Storage.Http.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Xunit;

namespace AI.Storage.Tests;

public class ContentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IContentV1Controller _client;

    public ContentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ProjectDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContextPool<ProjectDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            });
        });

        var httpClient = testFactory.CreateClient();
        _client = RestClient.For<IContentV1Controller>(httpClient);
    }

    [Fact]
    public async Task CreateContent_ShouldUploadFileSuccessfully()
    {
        // Arrange
        var fileName = "test.txt";
        var content = "This is a test file content";
        var multipartContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        multipartContent.Add(fileContent, "file", fileName);

        // Act
        var response = await _client.CreateContent(multipartContent, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ContentIds);
    }

    [Fact]
    public async Task GetContent_ShouldDownloadFileSuccessfully()
    {
        // Arrange
        var fileName = "test.txt";
        var content = "This is a test file content";
        var multipartContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        multipartContent.Add(fileContent, "file", fileName);

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
    }
}
