using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Application.Handlers.Commands;
using UrlShortener.Application.Handlers.Queries;
using UrlShortener.Infrastructure.BackgroundJobs;
using UrlShortener.Infrastructure.Services;
using UrlShotener.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.AddJob<UrlProcessorJob>(opts => opts.WithIdentity("UrlProcessorJob"));
    q.AddTrigger(opts => opts
        .ForJob("UrlProcessorJob")
        .WithIdentity("UrlProcessorJobTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>(); // Register the interface and its implementation

builder.Services.AddScoped<IUrlDeletionService, UrlDeletionService>();

builder.Services.AddScoped<UrlProcessorJob>(); // Register your background job

builder.Services.AddHttpContextAccessor();

builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024;
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(720) // L1 expiration
    };
});

builder.Services.AddScoped<IShortUrlGeneratorService, ShortUrlGeneratorService>();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateShortUrlCommandHandler).Assembly));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOriginalUrlQueryHandler).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();

            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();