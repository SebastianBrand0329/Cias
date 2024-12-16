using ApiCursosCias.Models.Context.Access;
using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Authorization;
using ApiCursosCias.Services.Helpers.CacheMemory;
using ApiCursosCias.Services.Helpers.Headers;
using ApiCursosCias.Services.Helpers.Middleware;
using ApiCursosCias.Services.Helpers.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiCursosCias.Services.Helpers.Extension;

public static class ExtensionServices
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddMvc(properties =>
        {
            properties.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
        });
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        MapperBootstrapper.Configure();
        services.AddSingleton(MapperBootstrapper.Instance);

        services.AddMvc().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = c =>
            {
                var errors = string.Join('|', c.ModelState.Values.Where(v => v.Errors.Count > 0)
                  .SelectMany(v => v.Errors)
                  .Select(v => v.ErrorMessage));

                var bad = new ResponseProblem
                {
                    Title = "One or more request errors occurred",
                    StatusCode = AppSettings.Settings.ErrorResult,
                    Status = StatusCodes.Status400BadRequest,
                    StatusMessage = errors
                }.Bad();
                return new BadRequestObjectResult(bad);
            };
        });

        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials();
        }));

        services.Configure<IISOptions>(options =>
        {
        });

        //LogginMiddelware
        var date = DateTime.Now;
        var dateFolder = @$"{date:yyyy}\{date.ToString("MMMM", CultureInfo.InvariantCulture)}\{date:dd}";
        string sourceDatabase = @$"{AppSettings.Settings.Logging.DefaultConnection}\{dateFolder}\{AppSettings.Settings.Logging.FolderDatabase}";
        if (!Directory.Exists(sourceDatabase))
            Directory.CreateDirectory(sourceDatabase);
        string dbName = @$"Data Source={sourceDatabase}\{AppSettings.Settings.Logging.DbName}";
        services.AddDbContext<LogDbContext>(options => options.UseSqlite(dbName));

        services.AddControllers();
        services.AddScoped<IHelpers, HelpersDao>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();
            //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inteligencia Movil API Agentes Pro Document", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, true);
            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "token",
                Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, Array.Empty<string>()
                    }
                });

            // add Basic Authentication
            var basicSecurityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Reference = new OpenApiReference { Id = "BasicAuth", Type = ReferenceType.SecurityScheme }
            };
            c.AddSecurityDefinition(basicSecurityScheme.Reference.Id, basicSecurityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {basicSecurityScheme, Array.Empty<string>()}
                });
        });

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
            config.ApiVersionReader = ApiVersionReader.Combine(
                new MediaTypeApiVersionReader("x-api-version"),
                new HeaderApiVersionReader("x-api-version")
            );
            config.ErrorResponses = new ApiVersioningErrorResponseProvider();
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        //NLog
        services.AddLogging(logging =>
        {
            logging.AddNLog();
            logging.AddNLogWeb();
        });

        services.AddSignalR();

        //validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //memory caching injection
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }

    public static void ConfigureHandler(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<LogDbContext>();
            context.Database.EnsureCreatedAsync();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Services.SetupNLogServiceLocator();
        app.ConfigureExceptionHandler();

        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var appSettings = AppSettings.Settings;
            var swg = provider.ApiVersionDescriptions.Where(w => appSettings.ApiVersion.Any(a => w.ApiVersion.ToString().Equals(a)));
            foreach (var info in swg)
            {
                options.SwaggerEndpoint($"../swagger/{info.GroupName}/swagger.json", info.ApiVersion.ToString());
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            }
        });

        app.UseSecurityHeaders(SecurityHeaders.GetHeaderPolicyCollection(!app.Environment.IsDevelopment()));

        // custom jwt auth middleware
        app.UseMiddleware<JwtMiddleware>();
        // global error handler
        app.UseMiddleware<ErrorHandler>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        //app.MapHub<MensajeriaHub>("/mensajeria");
    }

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    await context.Response.WriteAsync(new ResponseProblem
                    {
                        StatusCode = context.Response.StatusCode,
                        StatusMessage = contextFeature.Error.ToString()
                    }.ToString());
                }
            });
        });
    }
}