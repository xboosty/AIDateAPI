using Serilog;
using APICore.Filters;
using APICore.Middlewares;

using APICore.Data.Repository;
using APICore.Data.UoW;
using APICore.Services;
using APICore.Services.Impls;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace APICore;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Information()
                      .WriteTo.File("logs/apicore-.log", rollingInterval: RollingInterval.Day)
                      .CreateLogger();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.ConfigureI18N();
        services.ConfigureCors();
        services.AddMvc(config =>
        {
            config.Filters.Add(typeof(ApiValidationFilterAttribute));
            config.EnableEndpointRouting = false;
        }).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        services.ConfigureHsts();

        services.ConfigureDbContext(Configuration);
        services.ConfigureSwagger();
        services.ConfigureTokenAuth(Configuration);
        services.ConfigurePerformance();

        services.ConfigureHealthChecks(Configuration);
        services.ConfigureDetection();

        services.AddHttpContextAccessor();
        services.AddAutoMapper(typeof(Startup));

        // Adding the Azure blob clients as singletons
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(x => new BlobServiceClient(Configuration.GetConnectionString("Azure")));
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ISettingService, SettingService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ILogService, LogService>();
        services.AddTransient<IStorageService, StorageService>();
        services.AddTransient<ITwilioService, TwilioService>();
        services.AddTransient<IBlockService, BlockService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDetection();
        app.UseCors();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();

        if (env.IsProduction())
        {
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("v1/swagger.json", "API Core V1");
           });
        }
        else if (env.IsDevelopment())
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Core V1");
            });
        }

        #region Localization

        IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US")
            };
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };
        app.UseRequestLocalization(localizationOptions);

        var requestProvider = new RouteDataRequestCultureProvider();
        localizationOptions.RequestCultureProviders.Insert(0, requestProvider);
        var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        #endregion Localization

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseMiddleware(typeof(ErrorWrappingMiddleware));
        app.UseResponseCompression();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
            });
        });
    }
}