using ApiCursosCias.Services.Helpers.Middleware;
using ApiCursosCias.Services.Services;
using Quartz;

namespace ApiCursosCias.Job.Jobs;

[DisallowConcurrentExecution]
public class JobFupas : IJob
{
	private readonly ILogger<JobFupas> _logger;

	public JobFupas(ILogger<JobFupas> logger)
	{
		_logger = logger;
	}

	public Task Execute(IJobExecutionContext context)
	{
		using var _service = new AgendaService();

		Task.Run(async () =>
		{
			LoggerManager.logger.Info($"Inicio proceso Liberar Fupas");
			await _service.LiberarFupas();
			LoggerManager.logger.Info($"Fin proceso Liberar Fupas");
		});

		return Task.CompletedTask;
	}
}