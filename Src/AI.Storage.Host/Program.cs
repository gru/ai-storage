using System.Diagnostics;
using System.Reflection;
using AI.Storage;
using AI.Storage.Entities;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Serilog;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Destructure.ByTransforming<Exception>(ex => ex.Demystify())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddFeatureManagement();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("correlationId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
     
        if (context.Exception is ValidationException validationException)
        {
            context.ProblemDetails.Status = StatusCodes.Status400BadRequest;
            context.ProblemDetails.Title = "Validation error";
            context.ProblemDetails.Detail = "One or more validation errors occurred.";
            context.ProblemDetails.Extensions.Add("errors", validationException.Errors.Select(e => new 
            {
                e.PropertyName,
                e.ErrorMessage
            }));
        }
        
        if (builder.Environment.IsDevelopment())
        {
            var exceptionHandlerFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                context.ProblemDetails.Detail = exceptionHandlerFeature.Error.Demystify().ToString();
            }
        }
    };
});

builder.Services.AddDbContextPool<ProjectDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

var aiProjectAssembly = Assembly.Load("AI.Storage");
builder.Services.AddValidatorsFromAssembly(aiProjectAssembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStorageServices();

var app = builder.Build();

var versionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var swaggerGenOptions = app.Services.GetRequiredService<IOptions<SwaggerGenOptions>>();

foreach (var description in versionProvider.ApiVersionDescriptions)
{
    swaggerGenOptions.Value.SwaggerGeneratorOptions.SwaggerDocs.Add(
        description.GroupName,
        new OpenApiInfo
        {
            Title = $"AI.Storage API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        });
}

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
app.Services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value.IncludeXmlComments(xmlPath);

var s3Client = app.Services.GetRequiredService<IAmazonS3>();
var configuration = app.Services.GetRequiredService<IConfiguration>();
var bucketName = configuration["AWS:BucketName"];

try
{
    var bucketExists = AmazonS3Util.DoesS3BucketExist(s3Client, bucketName);
    if (!bucketExists)
    {
        var putBucketRequest = new PutBucketRequest
        {
            BucketName = bucketName,
            UseClientRegion = true
        };

        await s3Client.PutBucketAsync(putBucketRequest);
    }
}
catch (Exception exception)
{
  Log.Error(exception, "Error occured while trying to create bucket");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in versionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();