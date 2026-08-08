using System.Runtime;
using ApneScan.EntryPoints;
using ApneScan.Images.Gdi;
using ApneScan.ImportExport.Email.Mapi;
using ApneScan.Platform.Windows;
using ApneScan.Remoting.Worker;
using ApneScan.Scan;
using ApneScan.Scan.Internal.Twain;

namespace ApneScan.Worker;

static class Program
{
    /// <summary>
    /// The ApneScan.Worker.exe main method.
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
        var profilesPath = Path.Combine(Paths.AppData, "jit");
        Directory.CreateDirectory(profilesPath);
        ProfileOptimization.SetProfileRoot(profilesPath);
        ProfileOptimization.StartProfile("apnescan.worker.jit");

        // This ApneScan.App.Worker project doesn't follow the conventions of the rest of ApneScan as far as using EntryPoint
        // classes for everything. The reason is that we want to avoid pulling in extra dependencies as ApneScan.Worker.exe
        // is 32-bit and therefore requires a second copy of every single dependency we use.
        //
        // Thus the simplest solution is just to pull in a bit of code from ApneScan.Lib that has what we need
        // (pretty much only paths, logging, and the worker setup) and avoid using Autofac.
        var logger = NLogConfig.CreateLogger(() => NLogConfig.EnvDebugLogging);
        var messagePump = Win32MessagePump.Create();
        messagePump.Logger = logger;
        var scanningContext = new ScanningContext(new GdiImageContext());
        scanningContext.Logger = logger;
        var serviceImpl = new WorkerServiceImpl(scanningContext, new ThumbnailRenderer(scanningContext.ImageContext),
            new MapiWrapper(logger), new LocalTwainController(scanningContext));

        Trace.Listeners.Add(new NLog.NLogTraceListener());
        Invoker.Current = messagePump;
        TwainHandleManager.Factory = () => new Win32TwainHandleManager(messagePump);

        return CoreWorkerEntryPoint.Run(args, logger, serviceImpl, messagePump.RunMessageLoop, messagePump.Dispose);
    }
}