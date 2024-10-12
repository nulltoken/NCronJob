using NCronJob;
using System;

namespace NCronJob.TestProgram;

public class Sample : IJob
{
    public Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("In Job");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        return Task.CompletedTask;
    }
}
