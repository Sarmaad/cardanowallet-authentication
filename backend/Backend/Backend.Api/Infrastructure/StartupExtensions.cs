using Asp.Versioning;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Backend.Api.Infrastructure.Swagger;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.Api.Infrastructure;

public static class StartupExtensions
{
    public static WebApplicationBuilder ApiStandard(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            });



        builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });
        builder.Services.AddRouting(options => { options.LowercaseUrls = true; });
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressInferBindingSourcesForParameters = true;
        });
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(builder.Configuration["AllowedHosts"]!.Split(';'))
                .AllowAnyHeader().AllowAnyMethod();
        }));
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddProblemDetails();
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstitutionFormat = "VV";
                options.SubstituteApiVersionInUrl = true;

            });

        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenNewtonsoftSupport();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();

            options.MapType<decimal>(() => new() {Type = "number", Format = "decimal"});
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<Program>());



        return builder;
    }

    public static WebApplication AppStandard(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName);
            }
        });
        app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors();
        app.UseResponseCompression();

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapDefaultControllerRoute();
        });

        return app;
    }
}