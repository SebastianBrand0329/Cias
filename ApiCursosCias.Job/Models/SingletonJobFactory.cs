using Quartz;
using Quartz.Spi;

namespace ApiCursosCias.Job.Models;

public class SingletonJobFactory : IJobFactory
{
	private readonly IServiceProvider _serviceProvider;

	public SingletonJobFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
	{
		//LoggerManager.logger.Info($"NewJob start");
		return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
	}

	public void ReturnJob(IJob job)
	{
		// we let the DI container handler this
	}
}