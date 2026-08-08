using Autofac;
using CommandLine;
using ApneScan.Automation;
using ApneScan.EtoForms;
using ApneScan.Modules;
using ApneScan.Remoting.Worker;
using ApneScan.Scan;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point for ApneScan.Console.exe (on Windows) and "apnescan cli" (on Mac/Linux), the ApneScan CLI.
/// </summary>
public static class ConsoleEntryPoint
{
    public static int Run(string[] args, Module imageModule, Module platformModule)
    {
        // Parse the command-line arguments (and display help text if appropriate)
        var options = new Parser(settings =>
        {
            settings.HelpWriter = Console.Error;
            settings.CaseInsensitiveEnumValues = true;
        }).ParseArguments<AutomatedScanningOptions>(args).Value;
        if (options == null)
        {
            return 0;
        }

        // Initialize Autofac (the DI framework)
        var container = AutoFacHelper.FromModules(new CommonModule(), imageModule, platformModule,
            new ConsoleModule(options), new RecoveryModule(), new StaticInitModule());

        Paths.ClearTemp();

        // Start a pending worker process
        container.Resolve<IWorkerFactory>().Init(
            container.Resolve<ScanningContext>(),
            new WorkerFactoryInitOptions { StartSpareWorkers = false });

        // Run the scan automation logic
        var scanning = container.Resolve<AutomatedScanning>();

        if (options.Progress)
        {
            // We need to set up an Eto application in order to be able to display a progress GUI
            EtoPlatform.Current.InitializeForegroundApp();
            container.Resolve<CultureHelper>().SetCulturesFromConfig();

            var application = EtoPlatform.Current.CreateApplication();
            application.Initialized += (_, _) => scanning.Execute().ContinueWith(_ => application.Quit());
            Invoker.Current = new EtoInvoker(application);
            application.Run();
        }
        else
        {
            scanning.Execute().Wait();
        }

        return ((ConsoleErrorOutput) container.Resolve<ErrorOutput>()).HasError ? 1 : 0;
    }
}