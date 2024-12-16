using Quartz.Spi;
using Quartz;
using ApiCursosCias.Services.Helpers.Settings;
using System.Net.Sockets;
using ApiCursosCias.Services.Helpers.Middleware;

namespace ApiCursosCias.Job.Models;

public class QuartzHostedService : IHostedService
{
	private readonly ISchedulerFactory _schedulerFactory;
	private readonly IJobFactory _jobFactory;
	private readonly IEnumerable<JobSchedule> _jobSchedules;
	private readonly ILogger<QuartzHostedService> _logger;

	private readonly AppSettings _AppSettings = AppSettings.Settings;
	private TcpListener listener;
	private Socket listen;
	private Socket handler;

	public QuartzHostedService(
	ISchedulerFactory schedulerFactory,
	IEnumerable<JobSchedule> jobSchedules,
	IJobFactory jobFactory,
	ILogger<QuartzHostedService> logger)
	{
		_schedulerFactory = schedulerFactory;
		_jobSchedules = jobSchedules;
		_jobFactory = jobFactory;
		_logger = logger;
	}

	public IScheduler Scheduler { get; set; }

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
		Scheduler.JobFactory = _jobFactory;

		foreach (var jobSchedule in _jobSchedules)
		{
			var job = CreateJob(jobSchedule);
			var trigger = CreateTrigger(jobSchedule);

			await Scheduler.ScheduleJob(job, trigger, cancellationToken);
		}

		await Scheduler.Start(cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		LoggerManager.logger.Info($"StopAsync");
		await Scheduler?.Shutdown(cancellationToken);
	}

	private static ITrigger CreateTrigger(JobSchedule schedule)
	{
		return TriggerBuilder
			.Create()
			.WithSimpleSchedule(x => x.WithIntervalInSeconds(60) // cada 60 segundos
					.RepeatForever())
			.WithIdentity($"{schedule.JobType.FullName}.trigger")
			.Build();

		//return TriggerBuilder
		//    .Create()
		//    .WithIdentity($"{schedule.JobType.FullName}.trigger")
		//    .WithCronSchedule(schedule.CronExpression)
		//    .WithDescription(schedule.CronExpression)
		//    .Build();
	}

	private static IJobDetail CreateJob(JobSchedule schedule)
	{
		var jobType = schedule.JobType;
		return JobBuilder
			.Create(jobType)
			.WithIdentity(jobType.FullName)
			.WithDescription(jobType.Name)
			.Build();
	}
}