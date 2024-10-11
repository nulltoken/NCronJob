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

        builder.RegisterJobs(); // Complete building the NCronJobOptionBuilder

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

        return services;
    }

    // public static IServiceCollection AddNCronJob(
    //     this IServiceCollection services)
    //     {
    //         return AddNCronJob(services, (builder) => {});
    //     }

    public static IServiceCollection AddNCronJob(
        this IServiceCollection services,
        Action<NCronJobOptionBuilder, IServiceProvider> options)
    {
        // 4 is just an arbitrary multiplier based on system observed I/O, this could come from Configuration
        var settings = new ConcurrencySettings { MaxDegreeOfParallelism = Environment.ProcessorCount * 4 };
        var builder = new NCronJobOptionBuilder(services, settings);


                            // if (false) {
                            //         services.AddOptions<MySettings>().Configure(configure);
                            //         services.AddSingleton<IConfigureOptions<MySettings>, ConfigureMySettingsOptions>();
                            //         services.AddSingleton(resolver =>
                            //             resolver.GetRequiredService<IOptions<MySettings>>().Value);
                            // }

        if (options is not null)
        {
            services.TryAddSingleton((sp) =>
            {
                options(builder, sp);

                return new MySettings();
            });
        }

        builder.RegisterJobs(); // Complete building the NCronJobOptionBuilder

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

        return services;
    }

    // public static IApplicationBuilder UseNCronJob(
    //     this IApplicationBuilder services
    //     )
    // {
    //     var x = services.ApplicationServices.GetRequiredService<MySettings>();
    //     return services;
    // }
}

        public class ApplicationInsightsValidationService : IHostedService
        {
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly MySettings _options;
#pragma warning restore S4487 // Unread "private" fields should be removed

    public ApplicationInsightsValidationService(MySettings options)
            {
                _options = options;
            }

            public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }
public class MySettings
{
    public Action<NCronJobOptionBuilder, IServiceProvider> Configure { get; set; } = (builder, sp) => { };
}

public class ConfigureMySettingsOptions : IConfigureOptions<MySettings>
{
    private readonly IServiceProvider sp;
    private readonly NCronJobOptionBuilder builder;

    public ConfigureMySettingsOptions(NCronJobOptionBuilder builder, IServiceProvider sp)
    {
        this.sp = sp;
        this.builder = builder;
    }

    public void Configure(MySettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Configure(builder, sp);
    }
}
