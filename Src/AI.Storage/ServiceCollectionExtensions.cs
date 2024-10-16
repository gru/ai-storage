using Microsoft.Extensions.DependencyInjection;
using AI.Storage.Content;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace AI.Storage
{
    /// <summary>
    /// Provides extension methods for IServiceCollection to register project-specific services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Project Name specific services to the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddStorageServices(this IServiceCollection services)
        {
            services.AddScoped<ContentHandler>();
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var credentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"], 
                    configuration["AWS:SecretKey"]);
                var config = new AmazonS3Config
                {
                    ServiceURL = configuration["AWS:ServiceURL"]
                };
                return new AmazonS3Client(credentials, config);
            });
            
            return services;
        }
    }
}