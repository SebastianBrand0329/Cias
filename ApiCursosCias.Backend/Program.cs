using ApiCursosCias.Services.Helpers.Extension;
using ApiCursosCias.Services.Helpers.Settings;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .Build();

AppSettings.Settings = builder.Configuration.GetSection("appSettings").Get<AppSettings>().Decryt();
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.ConfigureServices();

var app = builder.Build();
app.ConfigureHandler();
app.Run();