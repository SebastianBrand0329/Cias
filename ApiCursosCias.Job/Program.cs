using ApiCursosCias.Job.Jobs;
using ApiCursosCias.Job.Models;
using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using NLog.Extensions.Logging;
using NLog.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = new ConfigurationBuilder()
	  .SetBasePath(Directory.GetCurrentDirectory())
	  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	  .Build();
AppSettings.Settings = builder.Configuration.GetSection("appSettings").Get<AppSettings>().Decryt();

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<QuartzHostedService>();
builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

builder.Services.AddSingleton<JobFupas>();

var appSettingsSection = AppSettings.Settings;
builder.Services.AddSingleton(new JobSchedule(
	jobType: typeof(JobFupas),
	cronExpression: appSettingsSection?.NotificationSettings.ScheduledTime
));

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
	endpoints.MapGet("/", async context =>
	{
		await context.Response.WriteAsync("Job Hosted Service Power BI!");
	});
});

app.Run();