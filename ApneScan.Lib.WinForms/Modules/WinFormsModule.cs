using Autofac;
using ApneScan.EtoForms.Ui;
using ApneScan.ImportExport;
using ApneScan.ImportExport.Email;
using ApneScan.ImportExport.Email.Mapi;
using ApneScan.Platform.Windows;

namespace ApneScan.Modules;

public class WinFormsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<WindowsApplicationLifecycle>().As<ApplicationLifecycle>();
        builder.RegisterType<PrintDocumentPrinter>().As<IScannedImagePrinter>();
        builder.RegisterType<WindowsServiceManager>().As<IOsServiceManager>().SingleInstance();
        builder.RegisterType<WindowsOpenWith>().As<IOpenWith>();
        builder.RegisterType<MapiEmailProvider>().As<IEmailProvider>().WithParameter("systemDefault", true);
        builder.RegisterType<MapiEmailClients>().As<ISystemEmailClients>();

        builder.RegisterType<WinFormsDesktopForm>().As<DesktopForm>();
        builder.RegisterType<WinFormsPreviewForm>().As<PreviewForm>();

        // TODO: Can we add a test for this?
        builder.RegisterBuildCallback(ctx =>
            Log.EventLogger = ctx.Resolve<WindowsEventLogger>());
    }
}