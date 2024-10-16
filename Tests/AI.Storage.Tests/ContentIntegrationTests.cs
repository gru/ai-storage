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
    public async Task CreateAndGetContent_ShouldSucceed()
    {
        var command = new CreateContentCommand
        {
            Name = "Test Content"
        };

        var id = await _client.CreateContent(command);
        var aggregate = await _client.GetContent(id);

        Assert.NotEqual(0, id);
        Assert.Equal(id, aggregate.Id);
        Assert.Equal(command.Name, aggregate.Name);
    }
}