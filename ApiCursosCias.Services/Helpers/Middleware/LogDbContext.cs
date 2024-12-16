using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Services.Helpers.Middleware;

public class LogDbContext : DbContext
{
    public DbSet<LogItem> Logs { get; set; }

    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }
}

public class LogItem
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Loglevel { get; set; }
    public string Callsite { get; set; }
    public string Message { get; set; }
    public string Request { get; set; }
    public string Response { get; set; }
}