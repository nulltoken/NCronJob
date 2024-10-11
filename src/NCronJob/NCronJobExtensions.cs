using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace NCronJob;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> to add cron jobs.
/// </summary>
public static class NCronJobExtensions
{
    /// <summary>
    /// Adds NCronJob services to the service container.
    /// </summary>
    /// <param name="services">The service collection used to register the services.</param>
    /// <param name="options">The builder to register jobs and other settings.</param>
    /// <example>
    /// To register a job that runs once every hour with a parameter and a handler that gets notified once the job is completed:
    /// <code>
    /// Services.AddNCronJob(options =>
    ///  .AddJob&lt;MyJob&gt;(c => c.WithCronExpression("0 * * * *").WithParameter("myParameter"))
    ///  .AddNotificationHandler&lt;MyJobHandler, MyJob&gt;());
    /// </code>
    /// </example>
    public static IServiceCollection AddNCronJob(
        this IServiceCollection services,
        Action<NCronJobOptionBuilder>? options = null)
    {
        // 4 is just an arbitrary multiplier based on system observed I/O, this could come from Configuration
        var settings = new ConcurrencySettings { MaxDegreeOfParallelism = Environment.ProcessorCount * 4 };
        var builder = new NCronJobOptionBuilder(services, settings);
        options?.Invoke(builder);

        RegisterCommonServices(services, settings);

        builder.RegisterJobs(); // Complete building the NCronJobOptionBuilder

        services.TryAddSingleton<MySettings>();

        return services;
    }

    public static IServiceCollection AddNCronJob(
        this IServiceCollection services,
        Action<NCronJobOptionBuilder, IServiceProvider> options)
    {
        // 4 is just an arbitrary multiplier based on system observed I/O, this could come from Configuration
        var settings = new ConcurrencySettings { MaxDegreeOfParallelism = Environment.ProcessorCount * 4 };
        var builder = new NCronJobOptionBuilder(services, settings);

        RegisterCommonServices(services, settings);

        services.TryAddSingleton((sp) =>
        {
            options(builder, sp);
            builder.RegisterJobs(); // Complete building the NCronJobOptionBuilder

            return new MySettings();
        });

        return services;
    }

    private static void RegisterCommonServices(
        IServiceCollection services,
        ConcurrencySettings settings)
    {
        services.TryAddSingleton(settings);
        services.AddHostedService<QueueWorker>();
        services.TryAddSingleton<JobRegistry>();
        services.TryAddSingleton<DynamicJobFactoryRegistry>();
        services.TryAddSingleton<IJobHistory, NoOpJobHistory>();
        services.TryAddSingleton<JobQueueManager>();
        services.TryAddSingleton<JobWorker>();
        services.TryAddSingleton<JobProcessor>();
        services.TryAddSingleton<JobExecutor>();
        services.TryAddSingleton<IRetryHandler, RetryHandler>();
        services.TryAddSingleton<IInstantJobRegistry, InstantJobRegistry>();
        services.TryAddSingleton<IRuntimeJobRegistry, RuntimeJobRegistry>();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<StartupJobManager>();
    }

}

internal class MySettings
{
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable S1186 // Methods should not be empty
    internal void IOnlyExistsToSilenceWarnings() { }
#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore CA1822 // Mark members as static
}
