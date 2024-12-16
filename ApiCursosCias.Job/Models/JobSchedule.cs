using ApiCursosCias.Services.Helpers.Settings;

namespace ApiCursosCias.Job.Models;

public class JobSchedule
{
	public JobSchedule(Type jobType, string cronExpression)
	{
		JobType = jobType;
		CronExpression = cronExpression;
	}

	public Type JobType { get; }
	public string CronExpression { get; }
	public AppSettings AppSettings { get; }
}