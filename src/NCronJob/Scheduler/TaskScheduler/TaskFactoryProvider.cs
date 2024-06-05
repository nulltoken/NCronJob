namespace NCronJob;

internal static class TaskFactoryProvider
{
    private static readonly DeterministicTaskScheduler TaskScheduler = new();

    private static readonly TaskFactory TaskFactory = new(CancellationToken.None,
        TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness | TaskCreationOptions.DenyChildAttach,
        TaskContinuationOptions.ExecuteSynchronously, TaskScheduler);

    public static TaskFactory GetTaskFactory() => TaskFactory;
}

