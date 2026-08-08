using Autofac;
using ApneScan.EtoForms;
using ApneScan.EtoForms.Ui;
using ApneScan.Modules;
using ApneScan.Remoting.Worker;
using ApneScan.Scan;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point for the main ApneScan executable.
/// </summary>
public static class GuiEntryPoint
{
    public static int Run(string[] args, Module imageModule, Module platformModule)
    {
        // Initialize Autofac (the DI framework)
        var container = AutoFacHelper.FromModules(
            new CommonModule(), imageModule, platformModule, new GuiModule(), new RecoveryModule(), new StaticInitModule());

        Paths.ClearTemp();

        // Parse the command-line arguments and see if we're doing something other than displaying the main form
        var lifecycle = container.Resolve<ApplicationLifecycle>();
        lifecycle.ParseArgs(args);
        lifecycle.ExitIfRedundant();

        EtoPlatform.Current.InitializeForegroundApp();

        // Set up basic application configuration
        container.Resolve<CultureHelper>().SetCulturesFromConfig();
        Trace.Listeners.Add(new ConsoleTraceListener());

        // Start a pending worker process
        container.Resolve<IWorkerFactory>().Init(container.Resolve<ScanningContext>());

        // Show the main form
        var application = EtoPlatform.Current.CreateApplication();
        Invoker.Current = new EtoInvoker(application);
        var formFactory = container.Resolve<IFormFactory>();
        var desktop = formFactory.Create<DesktopForm>();

        application.Run(desktop);
        return 0;
    }
}