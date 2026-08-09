using Autofac;
using Microsoft.Extensions.Logging;
using ApneScan.Modules;
using ApneScan.Remoting.Worker;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point for ApneScan.Worker.exe, an off-process worker.
///
/// ApneScan.Worker.exe runs in 32-bit mode for compatibility with 32-bit TWAIN drivers.
/// </summary>
public static class WorkerEntryPoint
{
    public static int Run(string[] args, Module imageModule, Action? run = null, Action? stop = null)
    {
            // Initialize Autofac (the DI framework)
            var container = AutoFacHelper.FromModules(
                new CommonModule(), imageModule, new WorkerModule(), new StaticInitModule());

            var logger = container.Resolve<ILogger>();
            var serviceImpl = container.Resolve<WorkerServiceImpl>();

            return CoreWorkerEntryPoint.Run(args, logger, serviceImpl, run, stop);
    }
}